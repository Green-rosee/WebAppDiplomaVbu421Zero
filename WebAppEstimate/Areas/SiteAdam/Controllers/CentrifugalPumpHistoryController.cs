using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppEstimate.Areas.SiteAdam.Models;
using WebAppEstimate.Areas.SiteAdam.Services;

namespace WebAppEstimate.Areas.SiteAdam.Controllers;

[Area("SiteAdam")]
[Authorize(Roles = "Admin,Adam")]
public class CentrifugalPumpHistoryController : Controller
{
    private readonly ICentrifugalPumpHistoryService _historyService;
    private readonly ICentrifugalPumpService _pumpService;


    public CentrifugalPumpHistoryController(
        ICentrifugalPumpHistoryService historyService,
        ICentrifugalPumpService pumpService)
    {
        _historyService = historyService;
        _pumpService = pumpService;
    }


    [HttpPost]
    public async Task<IActionResult> Save(CentrifugalPumpModel model)
    {
        await _historyService.SaveAsync(model);

        TempData["SuccessMessage"] = "Калькуляция сохранена в базе данных.";

        // Нужно снова заполнить список серий,
        // иначе select может оказаться пустым
        var result = await _pumpService.GetSeriesOptionsAsync();
        //-----
        if (result != null)
        {
            model.SeriesOptions = result;
        }
        else
        {
            model.SeriesOptions = new List<SelectListItem>();
        }

        //-----
        model.IsCalculated = true;
        return View(
            "~/Areas/SiteAdam/Views/CentrifugalPump/CentrifugalPumpForm.cshtml",
            model);
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var histories = await _historyService.GetAllAsync();

        return View(histories);
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppEstimate.Areas.SiteAdam.Models;
using WebAppEstimate.Areas.SiteAdam.Services;

namespace WebAppEstimate.Areas.SiteAdam.Controllers;

[Area("SiteAdam")]
[Authorize(Roles = "Admin,Adam")]
public class CentrifugalPumpHistoryController : Controller
{
    private readonly ICentrifugalPumpHistoryService _historyService;


    public CentrifugalPumpHistoryController(
        ICentrifugalPumpHistoryService historyService)
    {
        _historyService = historyService;
    }


    //---
    [HttpPost]
    public async Task<IActionResult> Save(CentrifugalPumpModel model)
    {
        await _historyService.SaveAsync(model);

        TempData["SuccessMessage"] = "Калькуляция сохранена в базе данных.";

        return RedirectToAction(
            "Calculate",
            "CentrifugalPump",
            new { area = "SiteAdam" });
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var histories = await _historyService.GetAllAsync();

        return View(histories);
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppEstimate.Areas.SiteAdam.Controllers;

[Area("SiteAdam")]
[Authorize(Roles = "Admin,Adam")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // GET
    public IActionResult Index()
    {
        _logger.LogDebug("метод View HomeController");
        return View();
    }

    //---GET CentrifugalPump
    public IActionResult AboutCentrifugalPump()
    {
        return RedirectToAction("Calculate", "CentrifugalPump");
        //
        //return View("/Areas/SiteAdam/Views/CentrifugalPump/CentrifugalPumpForm.cshtml");
    }
}
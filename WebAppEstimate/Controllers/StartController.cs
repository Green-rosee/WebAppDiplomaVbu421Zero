using Microsoft.AspNetCore.Mvc;

namespace WebAppEstimate.Controllers;

public class StartController : Controller
{
    // GET
    public IActionResult Start()
    {
        return View();
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppEstimate.Areas.SiteCerber.Controllers;

[Area("SiteCerber")]
[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    // GET: HomeController
    public ActionResult Index()
    {
        return View();
    }
}
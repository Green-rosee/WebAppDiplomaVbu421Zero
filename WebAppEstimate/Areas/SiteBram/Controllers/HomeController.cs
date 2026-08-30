using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppEstimate.Areas.SiteBram.Controllers;

public class HomeController : Controller
{
    // GET: HomeController
    [Area("SiteBram")]
    [Authorize(Roles = "Admin,Bram")]
    public ActionResult Index()
    {
        return View();
    }
}
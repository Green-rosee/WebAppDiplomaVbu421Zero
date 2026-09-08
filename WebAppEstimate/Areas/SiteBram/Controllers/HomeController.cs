using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppEstimate.Areas.SiteBram.Controllers;

[Area("SiteBram")]
    [Authorize(Roles = "Admin,Bram")]
public class HomeController : Controller
{
    // GET: HomeController
    public ActionResult Index()
    {
        return View();
    }
}
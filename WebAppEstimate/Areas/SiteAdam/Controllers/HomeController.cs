using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppEstimate.Areas.SiteAdam.Controllers;



/// <summary>
/// Главный контроллер раздела SiteAdam.
/// Отвечает за отображение стартовой страницы
/// и навигацию к расчёту центробежного насоса.
/// </summary>
[Area("SiteAdam")]
[Authorize(Roles = "Admin,Adam")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Создаёт контроллер главной страницы SiteAdam.
    /// </summary>
    /// <param name="logger">
    /// Сервис журналирования событий приложения.
    /// </param>
    public HomeController(
        ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Отображает главную страницу раздела SiteAdam.
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        // Записываем отладочное сообщение
        // о вызове главной страницы раздела.
        _logger.LogDebug(
            "Открыта главная страница SiteAdam.");

        return View();
    }

    /// <summary>
    /// Перенаправляет пользователя
    /// к форме расчёта центробежного насоса.
    /// </summary>
    [HttpGet]
    public IActionResult AboutCentrifugalPump()
    {
        return RedirectToAction(
            "Calculate",
            "CentrifugalPump");
    }
}











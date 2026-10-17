using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppEstimate.Areas.SiteAdam.Models;
using WebAppEstimate.Areas.SiteAdam.Services;

namespace WebAppEstimate.Areas.SiteAdam.Controllers;



/// <summary>
/// Контроллер расчёта центробежного насоса.
/// Отвечает за отображение формы, проверку введённых данных
/// и передачу расчёта в сервис бизнес-логики.
/// </summary>
[Area("SiteAdam")]
[Authorize(Roles = "Admin,Adam")]
public class CentrifugalPumpController : Controller
{
    private readonly ICentrifugalPumpService _pumpService;
    private readonly IValidator<CentrifugalPumpModel> _validator;

    /// <summary>
    /// Создаёт контроллер расчёта центробежного насоса.
    /// </summary>
    /// <param name="pumpService">
    /// Сервис расчёта параметров и трудоёмкости насоса.
    /// </param>
    /// <param name="validator">
    /// Валидатор входной модели FluentValidation.
    /// </param>
    public CentrifugalPumpController(
        ICentrifugalPumpService pumpService,
        IValidator<CentrifugalPumpModel> validator)
    {
        _pumpService = pumpService;
        _validator = validator;
    }

    /// <summary>
    /// Отображает главную страницу раздела центробежных насосов.
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Отображает форму расчёта центробежного насоса
    /// и загружает список доступных серий из базы данных.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Calculate()
    {
        var model =
            new CentrifugalPumpModel
            {
                SeriesOptions =
                    await _pumpService.GetSeriesOptionsAsync()
                    ?? new List<SelectListItem>()
            };

        return View(
            "CentrifugalPumpForm",
            model);
    }

    /// <summary>
    /// Обрабатывает отправленную форму,
    /// проверяет введённые данные и выполняет расчёт.
    /// </summary>
    /// <param name="model">
    /// Модель с параметрами центробежного насоса,
    /// введёнными пользователем.
    /// </param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Calculate(
        CentrifugalPumpModel model)
    {
        // Выполняем ручную проверку модели
        // через FluentValidation.
        var validationResult =
            await _validator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            // Переносим ошибки FluentValidation в ModelState,
            // чтобы Razor View мог отобразить их пользователю.
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(
                    error.PropertyName,
                    error.ErrorMessage);
            }

            // Повторно загружаем список серий,
            // так как он не передаётся обратно из HTML-формы.
            model.SeriesOptions =
                await _pumpService.GetSeriesOptionsAsync()
                ?? new List<SelectListItem>();

            return View(
                "CentrifugalPumpForm",
                model);
        }

        // Передаём проверенную модель
        // в сервис бизнес-логики для выполнения расчёта.
        model =
            await _pumpService
                .CalculateClosestHoursAsync(model);

        // Повторно заполняем список серий
        // для корректного отображения формы после расчёта.
        model.SeriesOptions =
            await _pumpService.GetSeriesOptionsAsync()
            ?? new List<SelectListItem>();

        return View(
            "CentrifugalPumpForm",
            model);
    }
}















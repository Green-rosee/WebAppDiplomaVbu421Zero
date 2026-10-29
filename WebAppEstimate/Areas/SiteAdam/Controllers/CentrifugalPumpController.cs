using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppEstimate.Areas.SiteAdam.Models;
using WebAppEstimate.Areas.SiteAdam.Services;

namespace WebAppEstimate.Areas.SiteAdam.Controllers;

/// <summary>
///     Контроллер расчёта центробежного насоса.
///     Отвечает за отображение формы,
///     загрузку доступных серий и нормативных наборов,
///     проверку введённых пользователем данных
///     и передачу расчёта в сервис бизнес-логики.
/// </summary>
[Area("SiteAdam")]
[Authorize(Roles = "Admin,Adam")]
public class CentrifugalPumpController : Controller
{
    private readonly ICentrifugalPumpService _pumpService;
    private readonly IValidator<CentrifugalPumpModel> _validator;

    /// <summary>
    ///     Создаёт контроллер расчёта центробежного насоса.
    /// </summary>
    /// <param name="pumpService">
    ///     Сервис расчёта параметров насоса
    ///     и получения справочных данных.
    /// </param>
    /// <param name="validator">
    ///     Валидатор модели расчёта FluentValidation.
    /// </param>
    public CentrifugalPumpController(
        ICentrifugalPumpService pumpService,
        IValidator<CentrifugalPumpModel> validator)
    {
        _pumpService = pumpService;
        _validator = validator;
    }

    /// <summary>
    ///     Отображает главную страницу
    ///     раздела центробежных насосов.
    /// </summary>
    /// <returns>
    ///     Представление главной страницы раздела.
    /// </returns>
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    ///     Отображает форму расчёта центробежного насоса.
    ///     Перед показом формы загружает из базы данных
    ///     доступные серии насосов и нормативные наборы.
    /// </summary>
    /// <returns>
    ///     Представление формы расчёта
    ///     с заполненными выпадающими списками.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> Calculate()
    {
        var model =
            new CentrifugalPumpModel
            {
                SeriesOptions =
                    await _pumpService.GetSeriesOptionsAsync()
                    ?? new List<SelectListItem>(),

                NormativeSetOptions =
                    await _pumpService.GetNormativeSetOptionsAsync()
                    ?? new List<SelectListItem>()
            };

        return View(
            "CentrifugalPumpForm",
            model);
    }

    /// <summary>
    ///     Обрабатывает отправленную форму расчёта.
    ///     Выполняет проверку введённых данных,
    ///     передаёт корректную модель
    ///     в сервис бизнес-логики
    ///     и возвращает рассчитанные значения
    ///     пользователю.
    /// </summary>
    /// <param name="model">
    ///     Модель с параметрами центробежного насоса,
    ///     выбранной серией
    ///     и нормативным набором.
    /// </param>
    /// <returns>
    ///     Представление формы с ошибками валидации
    ///     или с результатами выполненного расчёта.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Calculate(
        CentrifugalPumpModel model)
    {
        // Выполняем проверку модели
        // через FluentValidation.
        var validationResult =
            await _validator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            // Переносим ошибки FluentValidation
            // в ModelState,
            // чтобы Razor View мог отобразить их пользователю.
            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(
                    error.PropertyName,
                    error.ErrorMessage);

            // Выпадающие списки не возвращаются
            // из HTML-формы автоматически,
            // поэтому загружаем их повторно.
            model.SeriesOptions =
                await _pumpService.GetSeriesOptionsAsync()
                ?? new List<SelectListItem>();

            model.NormativeSetOptions =
                await _pumpService.GetNormativeSetOptionsAsync()
                ?? new List<SelectListItem>();

            return View(
                "CentrifugalPumpForm",
                model);
        }

        // Передаём проверенную модель
        // в сервис бизнес-логики.
        // Сервис подбирает ближайшие нормативные значения
        // для выбранной серии и нормативного набора
        // и рассчитывает итоговую трудоёмкость.
        model =
            await _pumpService
                .CalculateClosestHoursAsync(model);

        // После расчёта повторно загружаем
        // списки серий и нормативных наборов,
        // чтобы форма корректно отображалась.
        model.SeriesOptions =
            await _pumpService.GetSeriesOptionsAsync()
            ?? new List<SelectListItem>();

        model.NormativeSetOptions =
            await _pumpService.GetNormativeSetOptionsAsync()
            ?? new List<SelectListItem>();

        return View(
            "CentrifugalPumpForm",
            model);
    }
}


using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppEstimate.Areas.SiteAdam.Models;
using WebAppEstimate.Areas.SiteAdam.Services;

namespace WebAppEstimate.Areas.SiteAdam.Controllers;

[Area("SiteAdam")]
[Authorize(Roles = "Admin,Adam")]
public class CentrifugalPumpController : Controller
{
    private readonly ICentrifugalPumpService _pumpService;
    private readonly IValidator<CentrifugalPumpModel> _validator;

    // Внедряем сервис расчета и валидатор FluentValidation через конструктор
    public CentrifugalPumpController(
        ICentrifugalPumpService pumpService,
        IValidator<CentrifugalPumpModel> validator)
    {
        _pumpService = pumpService;
        _validator = validator;
    }

    // GET: push
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // 1. Отображение формы расчета
    [HttpGet]
    //[Authorize(Roles = "Admin,Adam")]
    public async Task<IActionResult> Calculate()
    {
        /*var model = new CentrifugalPumpModel
        {
            // Наполняем опции серий динамически из БД через сервис
            //SeriesOptions = await _pumpService.GetSeriesOptionsAsync()
            SeriesOptions = await _pumpService.GetSeriesOptionsAsync() ?? new List<SelectListItem>()
        };*/

        var model = new CentrifugalPumpModel();

        // Получаем данные из сервиса
        model.SeriesOptions = await _pumpService.GetSeriesOptionsAsync();

        // ЖЕСТКИЙ ТЕСТ: Если база пуста, принудительно пишем данные, чтобы исключить проблему с БД
        if (model.SeriesOptions == null || !model.SeriesOptions.Any())
            model.SeriesOptions = new List<SelectListItem>
            {
                new() { Value = "1", Text = "БАЗА ПУСТА: Тест Серия WA" },
                new() { Value = "2", Text = "БАЗА ПУСТА: Тест Серия NT" }
            };

        return View("CentrifugalPumpForm", model);
    }

    // 2. Обработка отправленной формы и выполнение подбора часов
    [HttpPost]
    [ValidateAntiForgeryToken]
    //[Authorize(Roles = "Admin,Adam")]
    public async Task<IActionResult> Calculate(CentrifugalPumpModel model)
    {
        // Ручной вызов FluentValidation
        var validationResult = await _validator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            // Переносим ошибки в ModelState для отображения в Razor View
            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            // Пересобираем выпадающий список, чтобы страница не упала
            //model.SeriesOptions = await _pumpService.GetSeriesOptionsAsync();
            model.SeriesOptions = await _pumpService.GetSeriesOptionsAsync() ?? new List<SelectListItem>();

            return View("CentrifugalPumpForm", model);
        }

        // --- ВЫЗОВ ВАШЕЙ БИЗНЕС-ЛОГИКИ ---
        // Сервис ищет ближайшие параметры по Math.Abs, умножает на количество и выставляет IsCalculated = true
        model = await _pumpService.CalculateClosestHoursAsync(model);
        model.SeriesOptions = await _pumpService.GetSeriesOptionsAsync() ?? new List<SelectListItem>();

        // Снова заполняем список серий для отображения на той же странице
        //model.SeriesOptions = await _pumpService.GetSeriesOptionsAsync();

        // Возвращаем ту же View: теперь блок с итоговыми часами отобразится внизу формы
        return View("CentrifugalPumpForm", model);
    }
}
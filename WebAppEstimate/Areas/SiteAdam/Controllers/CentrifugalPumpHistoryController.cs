using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppEstimate.Areas.SiteAdam.Models;
using WebAppEstimate.Areas.SiteAdam.Services;

namespace WebAppEstimate.Areas.SiteAdam.Controllers;

/// <summary>
///     Контроллер истории расчётов центробежных насосов.
///     Отвечает за сохранение выполненных расчётов,
///     просмотр истории,
///     удаление отдельных записей
///     и полную очистку истории.
/// </summary>
[Area("SiteAdam")]
[Authorize(Roles = "Admin,Adam")]
public class CentrifugalPumpHistoryController : Controller
{
    private readonly ICentrifugalPumpHistoryService _historyService;
    private readonly ICentrifugalPumpService _pumpService;

    /// <summary>
    ///     Создаёт контроллер истории расчётов
    ///     центробежных насосов.
    /// </summary>
    /// <param name="historyService">
    ///     Сервис работы с историей расчётов.
    /// </param>
    /// <param name="pumpService">
    ///     Сервис центробежных насосов,
    ///     используемый для повторной загрузки
    ///     списков серий и нормативных наборов.
    /// </param>
    public CentrifugalPumpHistoryController(
        ICentrifugalPumpHistoryService historyService,
        ICentrifugalPumpService pumpService)
    {
        _historyService = historyService;
        _pumpService = pumpService;
    }

    /// <summary>
    ///     Сохраняет выполненный расчёт
    ///     центробежного насоса в историю базы данных.
    ///     После сохранения повторно загружает
    ///     списки серий и нормативных наборов,
    ///     чтобы форма расчёта продолжала
    ///     корректно отображаться.
    /// </summary>
    /// <param name="model">
    ///     Модель с исходными параметрами
    ///     и результатами выполненного расчёта.
    /// </param>
    /// <returns>
    ///     Представление формы расчёта
    ///     с сохранённым состоянием результата.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        CentrifugalPumpModel model)
    {
        // Сохраняем текущий расчёт
        // через сервис истории.
        await _historyService.SaveAsync(model);

        // Передаём сообщение пользователю
        // после успешного сохранения.
        TempData["SuccessMessage"] =
            "Калькуляция сохранена в базе данных.";

        // Выпадающие списки не передаются
        // обратно из HTML-формы автоматически,
        // поэтому загружаем их повторно.
        model.SeriesOptions =
            await _pumpService.GetSeriesOptionsAsync()
            ?? new List<SelectListItem>();

        model.NormativeSetOptions =
            await _pumpService.GetNormativeSetOptionsAsync()
            ?? new List<SelectListItem>();

        // Сохраняем состояние рассчитанной модели,
        // чтобы результат продолжал отображаться
        // после сохранения в историю.
        model.IsCalculated = true;

        return View(
            "~/Areas/SiteAdam/Views/CentrifugalPump/CentrifugalPumpForm.cshtml",
            model);
    }

    /// <summary>
    ///     Удаляет одну запись
    ///     из истории расчётов.
    /// </summary>
    /// <param name="id">
    ///     Идентификатор удаляемой записи.
    /// </param>
    /// <returns>
    ///     Перенаправление на страницу истории.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id)
    {
        await _historyService.DeleteAsync(id);

        TempData["SuccessMessage"] =
            "Расчёт успешно удалён.";

        return RedirectToAction(
            nameof(Index));
    }

    /// <summary>
    ///     Полностью очищает историю
    ///     расчётов центробежных насосов.
    /// </summary>
    /// <returns>
    ///     Перенаправление на страницу истории.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAll()
    {
        await _historyService.DeleteAllAsync();

        TempData["SuccessMessage"] =
            "История расчётов полностью удалена.";

        return RedirectToAction(
            nameof(Index));
    }

    /// <summary>
    ///     Отображает историю сохранённых расчётов
    ///     центробежных насосов.
    ///     История также позволяет визуально сравнивать
    ///     результаты, выполненные по разным
    ///     нормативным наборам.
    /// </summary>
    /// <returns>
    ///     Представление со списком
    ///     сохранённых расчётов.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var histories =
            await _historyService.GetAllAsync();

        return View(histories);
    }
}


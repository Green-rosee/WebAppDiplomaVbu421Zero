using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppEstimate.Areas.SiteAdam.Models;
using WebAppEstimate.Areas.SiteAdam.Services;

namespace WebAppEstimate.Areas.SiteAdam.Controllers;

/// <summary>
///     Контроллер истории расчётов центробежных насосов.
///     Отвечает за сохранение, просмотр и удаление
///     результатов выполненных расчётов.
/// </summary>
[Area("SiteAdam")]
[Authorize(Roles = "Admin,Adam")]
public class CentrifugalPumpHistoryController : Controller
{
    private readonly ICentrifugalPumpHistoryService _historyService;
    private readonly ICentrifugalPumpService _pumpService;

    /// <summary>
    ///     Создаёт контроллер истории расчётов центробежных насосов.
    /// </summary>
    /// <param name="historyService">
    ///     Сервис работы с историей расчётов.
    /// </param>
    /// <param name="pumpService">
    ///     Сервис центробежных насосов,
    ///     используемый для повторного получения списка серий.
    /// </param>
    public CentrifugalPumpHistoryController(
        ICentrifugalPumpHistoryService historyService,
        ICentrifugalPumpService pumpService)
    {
        _historyService = historyService;
        _pumpService = pumpService;
    }

    /// <summary>
    ///     Сохраняет выполненный расчёт в историю базы данных.
    /// </summary>
    /// <param name="model">
    ///     Модель с параметрами и результатами расчёта.
    /// </param>
    [HttpPost]
    public async Task<IActionResult> Save(
        CentrifugalPumpModel model)
    {
        // Сохраняем текущий расчёт через сервис истории.
        await _historyService.SaveAsync(model);

        // Передаём сообщение пользователю
        // после успешного сохранения.
        TempData["SuccessMessage"] =
            "Калькуляция сохранена в базе данных.";

        // Повторно загружаем список серий,
        // так как он не передаётся обратно из HTML-формы.
        model.SeriesOptions =
            await _pumpService.GetSeriesOptionsAsync()
            ?? new List<SelectListItem>();

        model.NormativeSetOptions =
            await _pumpService.GetNormativeSetOptionsAsync()
            ?? new List<SelectListItem>();

        // Сохраняем состояние рассчитанной модели,
        // чтобы результат продолжал отображаться в форме.
        model.IsCalculated = true;

        return View(
            "~/Areas/SiteAdam/Views/CentrifugalPump/CentrifugalPumpForm.cshtml",
            model);
    }

    /// <summary>
    ///     Удаляет одну запись истории расчёта.
    /// </summary>
    /// <param name="id">
    ///     Идентификатор удаляемой записи.
    /// </param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _historyService.DeleteAsync(id);

        TempData["SuccessMessage"] =
            "Расчет успешно удален.";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    ///     Полностью очищает историю расчётов.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAll()
    {
        await _historyService.DeleteAllAsync();

        TempData["SuccessMessage"] =
            "История расчетов полностью удалена.";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    ///     Отображает историю сохранённых расчётов.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var histories =
            await _historyService.GetAllAsync();

        return View(histories);
    }
}
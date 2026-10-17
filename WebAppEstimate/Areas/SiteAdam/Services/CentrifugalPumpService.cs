using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;
using WebAppEstimate.Areas.SiteAdam.Models;

namespace WebAppEstimate.Areas.SiteAdam.Services;



/// <summary>
/// Сервис расчёта параметров центробежного насоса.
/// Получает исходные данные из базы и рассчитывает
/// трудоёмкость и стоимость изготовления.
/// </summary>
public class CentrifugalPumpService : ICentrifugalPumpService
{
    private readonly AppDbContextCentrifugalPump _dbContext;

    /// <summary>
    /// Создаёт сервис расчёта центробежного насоса.
    /// </summary>
    /// <param name="dbContext">
    /// Контекст базы данных SiteAdam.
    /// </param>
    public CentrifugalPumpService(
        AppDbContextCentrifugalPump dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Получает список доступных серий насосов
    /// для отображения в выпадающем списке интерфейса.
    /// </summary>
    /// <returns>
    /// Список элементов <see cref="SelectListItem"/>
    /// с идентификаторами и названиями серий.
    /// </returns>
    public async Task<List<SelectListItem>> GetSeriesOptionsAsync()
    {
        return await _dbContext.PumpSeries
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            })
            .ToListAsync();
    }

    /// <summary>
    /// Выполняет расчёт трудоёмкости и стоимости изготовления
    /// центробежных насосов на основе ближайших параметров из базы данных.
    /// </summary>
    /// <param name="model">
    /// Модель с выбранной серией и параметрами насоса.
    /// </param>
    /// <returns>
    /// Обновлённая модель с рассчитанными значениями
    /// трудоёмкости и стоимости.
    /// </returns>
    public async Task<CentrifugalPumpModel> CalculateClosestHoursAsync(
        CentrifugalPumpModel model)
    {
        // Ищем ближайшее значение рабочего колеса
        // для выбранной серии насоса.
        var closestImpeller =
            await _dbContext.PumpImpellers
                .Where(i =>
                    i.PumpSeriesId == model.SelectedSeriesId)
                .OrderBy(i =>
                    Math.Abs(i.Impeller - model.Impeller))
                .FirstOrDefaultAsync();

        // Ищем ближайшее значение массы
        // для выбранной серии насоса.
        var closestWeight =
            await _dbContext.PumpWeights
                .Where(w =>
                    w.PumpSeriesId == model.SelectedSeriesId)
                .OrderBy(w =>
                    Math.Abs(w.Weight - model.Weight))
                .FirstOrDefaultAsync();

        // Ищем ближайшее значение объёма
        // для выбранной серии насоса.
        var closestVolume =
            await _dbContext.PumpVolumes
                .Where(v =>
                    v.PumpSeriesId == model.SelectedSeriesId)
                .OrderBy(v =>
                    Math.Abs(v.Volume - model.Volume))
                .FirstOrDefaultAsync();

        // Сохраняем трудоёмкость каждой найденной операции.
        // Если подходящее значение отсутствует,
        // используется нулевое значение.
        model.HourImpeller =
            closestImpeller?.Hour ?? 0;

        model.HourWeight =
            closestWeight?.Hour ?? 0;

        model.HourVolume =
            closestVolume?.Hour ?? 0;

        // Рассчитываем трудоёмкость изготовления
        // одного насоса.
        var singlePumpSum =
            model.HourImpeller +
            model.HourWeight +
            model.HourVolume;

        // Рассчитываем общую трудоёмкость
        // для заданного количества насосов.
        model.TotalSumHour =
            Math.Round(
                singlePumpSum * model.Number,
                2);

        // Рассчитываем общую стоимость изготовления:
        // трудоёмкость одного насоса × количество × стоимость часа.
        model.TotalSumCostHour =
            Math.Round(
                (decimal)singlePumpSum *
                model.Number *
                model.CostHour,
                2);

        // Указываем интерфейсу, что расчёт выполнен
        // и результат можно отображать пользователю.
        model.IsCalculated = true;

        return model;
    }
}














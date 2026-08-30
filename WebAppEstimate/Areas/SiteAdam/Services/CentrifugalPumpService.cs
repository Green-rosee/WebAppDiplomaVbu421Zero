using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;
using WebAppEstimate.Areas.SiteAdam.Models;

namespace WebAppEstimate.Areas.SiteAdam.Services;

public class CentrifugalPumpService : ICentrifugalPumpService
{
    private readonly AppDbContextCentrifugalPump _dbContext;


    public CentrifugalPumpService(AppDbContextCentrifugalPump dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<List<SelectListItem>> GetSeriesOptionsAsync()
    {
        // 1. Пытаемся взять реальные серии из базы
        var options = await _dbContext.PumpSeries
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            })
            .ToListAsync();

        return options;
    }

    //---
    public async Task<CentrifugalPumpModel> CalculateClosestHoursAsync(CentrifugalPumpModel model)
    {
        // 1. Ищем ближайший Impeller для выбранной серии
        var closestImpeller = await _dbContext.PumpImpellers
            .Where(i => i.PumpSeriesId == model.SelectedSeriesId)
            .OrderBy(i => Math.Abs(i.Impeller - model.Impeller))
            .FirstOrDefaultAsync();

        // 2. Ищем ближайший Weight для выбранной серии
        var closestWeight = await _dbContext.PumpWeights
            .Where(w => w.PumpSeriesId == model.SelectedSeriesId)
            .OrderBy(w => Math.Abs(w.Weight - model.Weight))
            .FirstOrDefaultAsync();

        // 3. Ищем ближайший Volume для выбранной серии
        var closestVolume = await _dbContext.PumpVolumes
            .Where(v => v.PumpSeriesId == model.SelectedSeriesId)
            .OrderBy(v => Math.Abs(v.Volume - model.Volume))
            .FirstOrDefaultAsync();

        // Записываем чистые часы из БД в ваши новые свойства модели
        model.HourImpeller = closestImpeller?.Hour ?? 0;
        model.HourWeight = closestWeight?.Hour ?? 0;
        model.HourVolume = closestVolume?.Hour ?? 0;

        // Считаем сумму за один насос
        var singlePumpSum = model.HourImpeller +
                            model.HourWeight +
                            model.HourVolume;

        //---все количество Насосов
        model.TotalSumHour = Math.Round
            (singlePumpSum * model.Number, 2);

        //---все количество CostHour
        model.TotalSumCostHour = Math.Round(
            (decimal)singlePumpSum * model.Number * model.CostHour, 2);

        // Включаем флаг, что расчет готов для отображения
        model.IsCalculated = true;

        // Итоговое время с учетом количества насосов (Number)
        // Мы можем сохранить это значение в ViewBag, TempData или добавить поле TotalSum в модель
        return model;
    }
}
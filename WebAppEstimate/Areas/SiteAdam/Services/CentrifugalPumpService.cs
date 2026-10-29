using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;
using WebAppEstimate.Areas.SiteAdam.Models;

namespace WebAppEstimate.Areas.SiteAdam.Services;

/// <summary>
///     Сервис расчёта параметров центробежного насоса.
///     Получает нормативные данные из базы,
///     подбирает ближайшие значения для выбранной серии
///     и нормативного набора,
///     после чего рассчитывает трудоёмкость
///     и общую стоимость изготовления.
/// </summary>
public class CentrifugalPumpService : ICentrifugalPumpService
{
    private readonly AppDbContextCentrifugalPump _dbContext;

    /// <summary>
    ///     Создаёт сервис расчёта центробежного насоса.
    /// </summary>
    /// <param name="dbContext">
    ///     Контекст базы данных SiteAdam.
    /// </param>
    public CentrifugalPumpService(
        AppDbContextCentrifugalPump dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    ///     Получает список доступных серий насосов
    ///     для отображения в выпадающем списке интерфейса.
    /// </summary>
    /// <returns>
    ///     Список элементов <see cref="SelectListItem" />
    ///     с идентификаторами и названиями серий насосов.
    /// </returns>
    public async Task<List<SelectListItem>>
        GetSeriesOptionsAsync()
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
    ///     Выполняет расчёт трудоёмкости
    ///     и общей стоимости изготовления насосов.
    ///     Для каждого введённого параметра
    ///     подбирается ближайшее нормативное значение
    ///     в рамках выбранной серии насоса
    ///     и выбранного нормативного набора.
    /// </summary>
    /// <param name="model">
    ///     Модель с выбранной серией,
    ///     нормативным набором,
    ///     параметрами насоса,
    ///     количеством изделий
    ///     и стоимостью одного часа работ.
    /// </param>
    /// <returns>
    ///     Обновлённая модель
    ///     с рассчитанной трудоёмкостью,
    ///     общей трудоёмкостью
    ///     и итоговой стоимостью.
    /// </returns>
    public async Task<CentrifugalPumpModel>
        CalculateClosestHoursAsync(
            CentrifugalPumpModel model)
    {
        // Ищем ближайшее нормативное значение
        // диаметра рабочего колеса
        // для выбранной серии и нормативного набора.
        var closestImpeller =
            await _dbContext.PumpImpellers
                .Where(i =>
                    i.PumpSeriesId ==
                    model.SelectedSeriesId &&
                    i.PumpNormativeSetId ==
                    model.SelectedNormativeSetId)
                .OrderBy(i =>
                    Math.Abs(
                        i.Impeller -
                        model.Impeller))
                .FirstOrDefaultAsync();

        // Ищем ближайшее нормативное значение массы
        // для выбранной серии и нормативного набора.
        var closestWeight =
            await _dbContext.PumpWeights
                .Where(w =>
                    w.PumpSeriesId ==
                    model.SelectedSeriesId &&
                    w.PumpNormativeSetId ==
                    model.SelectedNormativeSetId)
                .OrderBy(w =>
                    Math.Abs(
                        w.Weight -
                        model.Weight))
                .FirstOrDefaultAsync();

        // Ищем ближайшее нормативное значение
        // производительности насоса
        // для выбранной серии и нормативного набора.
        var closestVolume =
            await _dbContext.PumpVolumes
                .Where(v =>
                    v.PumpSeriesId ==
                    model.SelectedSeriesId &&
                    v.PumpNormativeSetId ==
                    model.SelectedNormativeSetId)
                .OrderBy(v =>
                    Math.Abs(
                        v.Volume -
                        model.Volume))
                .FirstOrDefaultAsync();

        // Получаем трудоёмкость
        // для каждого найденного параметра.
        // Если нормативное значение отсутствует,
        // используется значение 0.
        model.HourImpeller =
            closestImpeller?.Hour ?? 0;

        model.HourWeight =
            closestWeight?.Hour ?? 0;

        model.HourVolume =
            closestVolume?.Hour ?? 0;

        // Рассчитываем суммарную трудоёмкость
        // изготовления одного насоса.
        var singlePumpSum =
            model.HourImpeller +
            model.HourWeight +
            model.HourVolume;

        // Рассчитываем общую трудоёмкость
        // для заданного количества насосов.
        model.TotalSumHour =
            Math.Round(
                singlePumpSum *
                model.Number,
                2);

        // Рассчитываем общую стоимость изготовления:
        //
        // трудоёмкость одного насоса
        // × количество насосов
        // × стоимость одного часа работ.
        model.TotalSumCostHour =
            Math.Round(
                (decimal)singlePumpSum *
                model.Number *
                model.CostHour,
                2);

        // Сообщаем интерфейсу,
        // что расчёт успешно выполнен
        // и результаты можно отображать пользователю.
        model.IsCalculated = true;

        return model;
    }

    /// <summary>
    ///     Получает список загруженных нормативных наборов
    ///     для отображения в выпадающем списке
    ///     на странице расчёта.
    /// </summary>
    /// <returns>
    ///     Список нормативных наборов,
    ///     содержащий их идентификаторы,
    ///     названия и имена исходных Excel-файлов.
    /// </returns>
    public async Task<List<SelectListItem>>
        GetNormativeSetOptionsAsync()
    {
        return await _dbContext.PumpNormativeSets
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name} ({x.FileName})"
            })
            .ToListAsync();
    }
}


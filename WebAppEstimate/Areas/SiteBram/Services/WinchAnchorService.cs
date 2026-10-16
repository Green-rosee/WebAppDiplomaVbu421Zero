using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

namespace WebAppEstimate.Areas.SiteBram.Services;


/// <summary>
/// Сервис подбора конфигурации якорной лебёдки.
/// Получает данные из базы и выбирает наиболее близкую
/// конфигурацию по заданным пользователем параметрам.
/// </summary>
public class WinchAnchorService : IWinchAnchorService
{
    private readonly AppDbContextWinchAnchor _context;

    /// <summary>
    /// Создаёт сервис подбора якорной лебёдки.
    /// </summary>
    /// <param name="context">
    /// Контекст базы данных SiteBram.
    /// </param>
    public WinchAnchorService(
        AppDbContextWinchAnchor context)
    {
        _context = context;
    }

    /// <summary>
    /// Получает доступные серии якорных лебёдок
    /// из базы данных.
    /// </summary>
    /// <returns>
    /// Список серий, отсортированный по названию.
    /// </returns>
    public async Task<List<WinchAnchorSeries>> GetSeriesAsync()
    {
        return await _context.WinchAnchorSeries
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Выполняет подбор наиболее подходящей конфигурации
    /// якорной лебёдки для выбранной серии.
    /// </summary>
    /// <param name="seriesId">
    /// Идентификатор выбранной серии лебёдки.
    /// </param>
    /// <param name="valueKg">
    /// Требуемое значение параметра массы.
    /// </param>
    /// <param name="valueMm">
    /// Требуемое значение линейного параметра.
    /// </param>
    /// <param name="valueKgMm">
    /// Требуемое комбинированное значение параметра.
    /// </param>
    /// <returns>
    /// Наиболее близкая конфигурация либо null,
    /// если для выбранной серии конфигурации отсутствуют.
    /// </returns>
    public async Task<WinchAnchorDesign?> FindDesignAsync(
        Guid seriesId,
        int valueKg,
        int valueMm,
        int valueKgMm)
    {
        // Получаем все доступные конфигурации
        // для выбранной серии.
        var designs =
            await _context.WinchAnchorDesigns
                .Where(x =>
                    x.WinchAnchorSeriesId == seriesId)
                .ToListAsync();

        if (designs.Count == 0)
            return null;

        // Для каждой конфигурации рассчитывается
        // суммарное относительное отклонение
        // от параметров, заданных пользователем.
        //
        // Нормализация через деление на исходное значение
        // позволяет сравнивать параметры разного масштаба.
        return designs
            .OrderBy(x =>
                Math.Abs(
                    (double)x.ValueKg - valueKg)
                    / Math.Max(valueKg, 1)
                +
                Math.Abs(
                    (double)x.ValueMm - valueMm)
                    / Math.Max(valueMm, 1)
                +
                Math.Abs(
                    (double)x.ValueKgMm - valueKgMm)
                    / Math.Max(valueKgMm, 1))
            .FirstOrDefault();
    }
}






















using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;
using WebAppEstimate.Areas.SiteAdam.Data.Entity.CentrifugalPumps;
using WebAppEstimate.Areas.SiteAdam.Models;

namespace WebAppEstimate.Areas.SiteAdam.Services.ExcelPumps;

/// <summary>
///     Сервис импорта нормативных данных центробежных насосов
///     из Excel-файла в базу данных.
/// </summary>
public class CentrifugalPumpExcelImportService
    : ICentrifugalPumpExcelImportService
{
    private readonly AppDbContextCentrifugalPump _context;

    /// <summary>
    ///     Создаёт сервис импорта Excel-файлов.
    /// </summary>
    public CentrifugalPumpExcelImportService(
        AppDbContextCentrifugalPump context)
    {
        _context = context;
    }

    /// <summary>
    ///     Импортирует нормативный набор из Excel-файла.
    /// </summary>
    public async Task ImportAsync(
        Stream stream,
        string dataSetName,
        string fileName)
    {
        var exists = await _context.PumpNormativeSets
            .AnyAsync(x =>
                x.Name == dataSetName ||
                x.FileName == fileName);

        if (exists)
            throw new InvalidOperationException(
                $"Набор нормативов '{dataSetName}' " +
                $"или файл '{fileName}' уже загружен.");

        // Создаём отдельный нормативный набор,
        // к которому будут привязаны импортированные данные.
        var normativeSet = new PumpNormativeSet
        {
            Name = dataSetName,
            FileName = fileName,
            CreatedAt = DateTime.UtcNow
        };

        _context.PumpNormativeSets.Add(normativeSet);

        await _context.SaveChangesAsync();

        // ExcelDataReader читает Excel последовательно,
        // без загрузки всей книги в DataSet.
        using var reader =
            ExcelReaderFactory.CreateReader(stream);

        var isHeader = true;

        // Кэш серий нужен, чтобы не выполнять запрос к БД
        // для каждой строки Excel.
        var seriesCache =
            new Dictionary<string, PumpSeries>(
                StringComparer.OrdinalIgnoreCase);

        while (reader.Read())
        {
            // Первая строка содержит названия колонок.
            if (isHeader)
            {
                isHeader = false;
                continue;
            }

            // Пропускаем полностью пустые строки.
            if (reader.FieldCount < 7 ||
                reader.IsDBNull(0))
                continue;

            var seriesName =
                reader.GetValue(0)?.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(seriesName)) continue;

            var impeller =
                Convert.ToInt32(reader.GetValue(1));

            var hourImpeller =
                Convert.ToDouble(reader.GetValue(2));

            var weight =
                Convert.ToInt32(reader.GetValue(3));

            var hourWeight =
                Convert.ToDouble(reader.GetValue(4));

            var volume =
                Convert.ToInt32(reader.GetValue(5));

            var hourVolume =
                Convert.ToDouble(reader.GetValue(6));

            // Получаем или создаём серию насоса.
            if (!seriesCache.TryGetValue(
                    seriesName,
                    out var series))
            {
                series =
                    await _context.PumpSeries
                        .FirstOrDefaultAsync(x => x.Name == seriesName);

                if (series == null)
                {
                    series = new PumpSeries
                    {
                        Name = seriesName
                    };

                    _context.PumpSeries.Add(series);

                    await _context.SaveChangesAsync();
                }

                seriesCache[seriesName] = series;
            }

            // Добавляем норматив рабочего колеса.
            _context.PumpImpellers.Add(
                new PumpImpeller
                {
                    PumpSeriesId = series.Id,
                    PumpNormativeSetId = normativeSet.Id,
                    Impeller = impeller,
                    Hour = hourImpeller
                });

            // Добавляем норматив массы.
            _context.PumpWeights.Add(
                new PumpWeight
                {
                    PumpSeriesId = series.Id,
                    PumpNormativeSetId = normativeSet.Id,
                    Weight = weight,
                    Hour = hourWeight
                });

            // Добавляем норматив объёма.
            _context.PumpVolumes.Add(
                new PumpVolume
                {
                    PumpSeriesId = series.Id,
                    PumpNormativeSetId = normativeSet.Id,
                    Volume = volume,
                    Hour = hourVolume
                });
        }

        // Сохраняем все импортированные нормативные данные.
        await _context.SaveChangesAsync();
    }
}
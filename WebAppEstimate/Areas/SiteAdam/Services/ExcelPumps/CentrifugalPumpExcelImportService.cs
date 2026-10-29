using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;
using WebAppEstimate.Areas.SiteAdam.Data.Entity.CentrifugalPumps;
using WebAppEstimate.Areas.SiteAdam.Models;

namespace WebAppEstimate.Areas.SiteAdam.Services.ExcelPumps;

/// <summary>
///     Сервис импорта нормативных данных центробежных насосов
///     из Excel-файлов в базу данных.
///     Каждый импорт создаёт отдельный нормативный набор,
///     с которым связываются значения рабочего колеса,
///     массы и объёма.
/// </summary>
public class CentrifugalPumpExcelImportService
    : ICentrifugalPumpExcelImportService
{
    private readonly AppDbContextCentrifugalPump _context;

    /// <summary>
    ///     Создаёт сервис импорта нормативных Excel-файлов.
    /// </summary>
    /// <param name="context">
    ///     Контекст базы данных SiteAdam.
    /// </param>
    public CentrifugalPumpExcelImportService(
        AppDbContextCentrifugalPump context)
    {
        _context = context;
    }

    /// <summary>
    ///     Импортирует нормативный набор из Excel-файла
    ///     и сохраняет его данные в базе данных.
    ///     Перед импортом выполняется проверка,
    ///     не был ли ранее загружен набор с таким названием
    ///     или Excel-файл с таким именем.
    /// </summary>
    /// <param name="stream">
    ///     Поток с содержимым Excel-файла.
    /// </param>
    /// <param name="dataSetName">
    ///     Название нормативного набора,
    ///     указанное пользователем.
    /// </param>
    /// <param name="fileName">
    ///     Имя исходного Excel-файла.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Возникает, если набор с таким названием
    ///     или файл с таким именем уже были импортированы.
    /// </exception>
    public async Task ImportAsync(
        Stream stream,
        string dataSetName,
        string fileName)
    {
        // Проверяем защиту от повторной загрузки.
        // Дубликатом считается совпадение названия набора
        // или имени исходного Excel-файла.
        var exists =
            await _context.PumpNormativeSets
                .AnyAsync(x =>
                    x.Name == dataSetName ||
                    x.FileName == fileName);

        if (exists)
            throw new InvalidOperationException(
                $"Набор нормативов '{dataSetName}' " +
                $"или файл '{fileName}' уже загружен.");

        // Создаём отдельный нормативный набор,
        // к которому будут привязаны все импортированные данные.
        var normativeSet =
            new PumpNormativeSet
            {
                Name = dataSetName,
                FileName = fileName,
                CreatedAt = DateTime.UtcNow
            };

        _context.PumpNormativeSets
            .Add(normativeSet);

        // Сохраняем набор заранее,
        // чтобы получить его Id для внешних ключей.
        await _context.SaveChangesAsync();

        // ExcelDataReader читает Excel последовательно,
        // без загрузки всей книги в DataSet.
        using var reader =
            ExcelReaderFactory.CreateReader(stream);

        var isHeader = true;

        // Кэш уже найденных серий.
        // Позволяет не выполнять запрос к базе
        // для каждой строки Excel-файла.
        var seriesCache =
            new Dictionary<string, PumpSeries>(
                StringComparer.OrdinalIgnoreCase);

        while (reader.Read())
        {
            // Первая строка Excel содержит названия колонок,
            // поэтому её пропускаем.
            if (isHeader)
            {
                isHeader = false;
                continue;
            }

            // Для импорта ожидается минимум семь колонок.
            // Также пропускаем строки без имени серии.
            if (reader.FieldCount < 7 ||
                reader.IsDBNull(0))
                continue;

            var seriesName =
                reader.GetValue(0)?
                    .ToString()?
                    .Trim();

            if (string.IsNullOrWhiteSpace(seriesName)) continue;

            // Читаем нормативные значения из текущей строки Excel.
            var impeller =
                Convert.ToInt32(
                    reader.GetValue(1));

            var hourImpeller =
                Convert.ToDouble(
                    reader.GetValue(2));

            var weight =
                Convert.ToInt32(
                    reader.GetValue(3));

            var hourWeight =
                Convert.ToDouble(
                    reader.GetValue(4));

            var volume =
                Convert.ToInt32(
                    reader.GetValue(5));

            var hourVolume =
                Convert.ToDouble(
                    reader.GetValue(6));

            // Получаем серию насоса из локального кэша.
            // Если её ещё нет в кэше,
            // выполняем поиск в базе данных.
            if (!seriesCache.TryGetValue(
                    seriesName,
                    out var series))
            {
                series =
                    await _context.PumpSeries
                        .FirstOrDefaultAsync(x =>
                            x.Name == seriesName);

                // Если такой серии ещё нет,
                // создаём новую запись.
                if (series == null)
                {
                    series =
                        new PumpSeries
                        {
                            Name = seriesName
                        };

                    _context.PumpSeries
                        .Add(series);

                    // Сохраняем серию,
                    // чтобы получить её Id
                    // для нормативных записей.
                    await _context.SaveChangesAsync();
                }

                seriesCache[seriesName] =
                    series;
            }

            // Добавляем норматив рабочего колеса.
            _context.PumpImpellers.Add(
                new PumpImpeller
                {
                    PumpSeriesId =
                        series.Id,

                    PumpNormativeSetId =
                        normativeSet.Id,

                    Impeller =
                        impeller,

                    Hour =
                        hourImpeller
                });

            // Добавляем норматив массы.
            _context.PumpWeights.Add(
                new PumpWeight
                {
                    PumpSeriesId =
                        series.Id,

                    PumpNormativeSetId =
                        normativeSet.Id,

                    Weight =
                        weight,

                    Hour =
                        hourWeight
                });

            // Добавляем норматив объёма.
            _context.PumpVolumes.Add(
                new PumpVolume
                {
                    PumpSeriesId =
                        series.Id,

                    PumpNormativeSetId =
                        normativeSet.Id,

                    Volume =
                        volume,

                    Hour =
                        hourVolume
                });
        }

        // Сохраняем все импортированные нормативные записи
        // одним вызовом после обработки Excel-файла.
        await _context.SaveChangesAsync();
    }
}


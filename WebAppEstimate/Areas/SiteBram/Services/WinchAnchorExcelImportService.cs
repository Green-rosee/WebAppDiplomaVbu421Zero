using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

namespace WebAppEstimate.Areas.SiteBram.Services;



/// <summary>
/// Импортирует исходные данные для расчёта якорных лебёдок
/// из Excel-файла в базу данных SiteBram.
/// </summary>
public class WinchAnchorExcelImportService : IWinchExcelImportService
{
    private readonly AppDbContextWinchAnchor _context;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Создаёт сервис импорта данных из Excel.
    /// </summary>
    /// <param name="context">
    /// Контекст базы данных SiteBram.
    /// </param>
    /// <param name="environment">
    /// Информация о текущем окружении приложения
    /// и корневом каталоге проекта.
    /// </param>
    public WinchAnchorExcelImportService(
        AppDbContextWinchAnchor context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    /// <summary>
    /// Выполняет полный импорт исходных данных
    /// из Excel-файла в базу данных.
    /// </summary>
    public async Task ImportAsync()
    {
        Console.WriteLine(
            "=== Winch Excel import started ===");

        // Формируем абсолютный путь к Excel-файлу
        // относительно корневой папки приложения.
        var filePath = Path.Combine(
            _environment.ContentRootPath,
            "Areas",
            "SiteBram",
            "ExcelFileLoad",
            "WinchAnchorExcel",
            "WinchAnchorTest.xlsx"
        );

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"Excel file not found: {filePath}");
        }

        using var workbook =
            new XLWorkbook(filePath);

        // Импортируем справочные данные.
        await ImportSeriesAsync(workbook);
        await ImportWeightsAsync(workbook);
        await ImportShaftsAsync(workbook);
        await ImportChainsAsync(workbook);

        // Сохраняем связанные сущности,
        // чтобы получить их идентификаторы.
        await _context.SaveChangesAsync();

        // После сохранения справочников
        // формируем итоговые комбинации конструкции.
        await ImportDesignsAsync(workbook);

        Console.WriteLine(
            "=== Winch Excel import finished ===");
    }

    /// <summary>
    /// Импортирует список серий якорных лебёдок
    /// из листа Excel "Series".
    /// </summary>
    private async Task ImportSeriesAsync(
        XLWorkbook workbook)
    {
        var worksheet =
            workbook.Worksheet("Series");

        // Первая строка считается заголовком.
        var rows =
            worksheet.RowsUsed().Skip(1);

        // Получаем уже существующие серии,
        // чтобы не добавлять дубликаты.
        var existingNames =
            await _context.WinchAnchorSeries
                .Select(x => x.Name)
                .ToListAsync();

        foreach (var row in rows)
        {
            var seriesName =
                row.Cell(1)
                    .GetString()
                    .Trim();

            if (string.IsNullOrWhiteSpace(seriesName))
                continue;

            if (existingNames.Contains(seriesName))
                continue;

            _context.WinchAnchorSeries.Add(
                new WinchAnchorSeries
                {
                    Name = seriesName
                });

            existingNames.Add(seriesName);
        }

        // Сохраняем серии,
        // чтобы их Id можно было использовать дальше.
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Импортирует весовые параметры
    /// из листа Excel "Weight".
    /// </summary>
    private async Task ImportWeightsAsync(
        XLWorkbook workbook)
    {
        var worksheet =
            workbook.Worksheet("Weight");

        var rows =
            worksheet.RowsUsed().Skip(1);

        // Создаём словарь "Название серии -> Id",
        // чтобы быстро связывать строки Excel с сущностями БД.
        var seriesDictionary =
            await _context.WinchAnchorSeries
                .ToDictionaryAsync(
                    x => x.Name,
                    x => x.Id);

        foreach (var row in rows)
        {
            var seriesName =
                row.Cell(1)
                    .GetString()
                    .Trim();

            if (string.IsNullOrWhiteSpace(seriesName))
                continue;

            var valueKg =
                row.Cell(2).GetValue<int>();

            var hour =
                row.Cell(3).GetValue<double>();

            if (!seriesDictionary.TryGetValue(
                    seriesName,
                    out var seriesId))
            {
                continue;
            }

            var weight =
                new WinchWeight
                {
                    Name = seriesName,
                    ValueKg = valueKg,
                    Hour = hour,
                    WinchAnchorSeriesId = seriesId
                };

            _context.WinchWeights.Add(weight);
        }
    }

    /// <summary>
    /// Импортирует параметры вала
    /// из листа Excel "Shaft".
    /// </summary>
    private async Task ImportShaftsAsync(
        XLWorkbook workbook)
    {
        var worksheet =
            workbook.Worksheet("Shaft");

        var rows =
            worksheet.RowsUsed().Skip(1);

        var seriesDictionary =
            await _context.WinchAnchorSeries
                .ToDictionaryAsync(
                    x => x.Name,
                    x => x.Id);

        foreach (var row in rows)
        {
            var seriesName =
                row.Cell(1)
                    .GetString()
                    .Trim();

            if (string.IsNullOrWhiteSpace(seriesName))
                continue;

            var valueMm =
                row.Cell(2).GetValue<int>();

            var hour =
                row.Cell(3).GetValue<double>();

            if (!seriesDictionary.TryGetValue(
                    seriesName,
                    out var seriesId))
            {
                continue;
            }

            var shaft =
                new WinchShaft
                {
                    Name = seriesName,
                    ValueMm = valueMm,
                    Hour = hour,
                    WinchAnchorSeriesId = seriesId
                };

            _context.WinchShafts.Add(shaft);
        }
    }

    /// <summary>
    /// Импортирует параметры цепи
    /// из листа Excel "Chain".
    /// </summary>
    private async Task ImportChainsAsync(
        XLWorkbook workbook)
    {
        var worksheet =
            workbook.Worksheet("Chain");

        var rows =
            worksheet.RowsUsed().Skip(1);

        var seriesDictionary =
            await _context.WinchAnchorSeries
                .ToDictionaryAsync(
                    x => x.Name,
                    x => x.Id);

        foreach (var row in rows)
        {
            var seriesName =
                row.Cell(1)
                    .GetString()
                    .Trim();

            if (string.IsNullOrWhiteSpace(seriesName))
                continue;

            var valueKgMm =
                row.Cell(2).GetValue<int>();

            var hour =
                row.Cell(3).GetValue<double>();

            if (!seriesDictionary.TryGetValue(
                    seriesName,
                    out var seriesId))
            {
                continue;
            }

            var chain =
                new WinchChain
                {
                    Name = seriesName,
                    ValueKgMm = valueKgMm,
                    Hour = hour,
                    WinchAnchorSeriesId = seriesId
                };

            _context.WinchChains.Add(chain);
        }
    }

    /// <summary>
    /// Формирует итоговые варианты конструкции
    /// на основе данных листа Excel "Design".
    /// </summary>
    private async Task ImportDesignsAsync(
        XLWorkbook workbook)
    {
        var worksheet =
            workbook.Worksheet("Design");

        var rows =
            worksheet.RowsUsed().Skip(1);

        foreach (var row in rows)
        {
            var seriesName =
                row.Cell(1)
                    .GetString()
                    .Trim();

            if (string.IsNullOrWhiteSpace(seriesName))
                continue;

            var valueKg =
                row.Cell(2).GetValue<int>();

            var valueMm =
                row.Cell(3).GetValue<int>();

            var valueKgMm =
                row.Cell(4).GetValue<int>();

            // Находим серию, к которой относится строка Excel.
            var series =
                await _context.WinchAnchorSeries
                    .FirstOrDefaultAsync(
                        x => x.Name == seriesName);

            if (series == null)
                continue;

            // Находим связанные параметры конструкции.
            var weight =
                await _context.WinchWeights
                    .FirstOrDefaultAsync(x =>
                        x.WinchAnchorSeriesId == series.Id &&
                        x.ValueKg == valueKg);

            var shaft =
                await _context.WinchShafts
                    .FirstOrDefaultAsync(x =>
                        x.WinchAnchorSeriesId == series.Id &&
                        x.ValueMm == valueMm);

            var chain =
                await _context.WinchChains
                    .FirstOrDefaultAsync(x =>
                        x.WinchAnchorSeriesId == series.Id &&
                        x.ValueKgMm == valueKgMm);

            // Если хотя бы один из обязательных элементов
            // отсутствует, конструкция не создаётся.
            if (weight == null ||
                shaft == null ||
                chain == null)
            {
                continue;
            }

            // Проверяем, что такая комбинация
            // ещё не существует в базе данных.
            var designExists =
                await _context.WinchAnchorDesigns
                    .AnyAsync(x =>
                        x.WinchAnchorSeriesId == series.Id &&
                        x.WinchWeightId == weight.Id &&
                        x.WinchShaftId == shaft.Id &&
                        x.WinchChainId == chain.Id);

            if (designExists)
                continue;

            var design =
                new WinchAnchorDesign
                {
                    Name =
                        $"{seriesName}-{valueKg}-{valueMm}-{valueKgMm}",

                    ValueKg = valueKg,
                    ValueMm = valueMm,
                    ValueKgMm = valueKgMm,

                    // Общее время рассчитывается
                    // как сумма трудоёмкости всех компонентов.
                    Hour = Math.Round(
                        weight.Hour +
                        shaft.Hour +
                        chain.Hour,
                        2),

                    WinchAnchorSeriesId = series.Id,
                    WinchWeightId = weight.Id,
                    WinchShaftId = shaft.Id,
                    WinchChainId = chain.Id
                };

            _context.WinchAnchorDesigns.Add(design);
        }

        await _context.SaveChangesAsync();
    }
}
















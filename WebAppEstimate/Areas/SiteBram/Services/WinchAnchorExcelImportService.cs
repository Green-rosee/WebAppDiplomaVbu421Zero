using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

namespace WebAppEstimate.Areas.SiteBram.Services;

public class WinchAnchorExcelImportService : IWinchExcelImportService
{
    private readonly AppDbContextWinchAnchor _context;
    private readonly IWebHostEnvironment _environment;

    public WinchAnchorExcelImportService(AppDbContextWinchAnchor context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task ImportAsync()
    {
        Console.WriteLine("=== Winch Excel import started ===");
        var filePath = Path.Combine(
            _environment.ContentRootPath,
            "Areas",
                "SiteBram",
                    "ExcelFileLoad",
                        "WinchAnchorExcel",
                            "WinchAnchorTest.xlsx"
        );
        
        /*Console.WriteLine($"Excel path: {filePath}");
        Console.WriteLine($"Excel exists: {File.Exists(filePath)}");*/

        using var workbook = new XLWorkbook(filePath);

        await ImportSeriesAsync(workbook);
            await ImportWeightsAsync(workbook);
            
            /*Console.WriteLine(
                $"Weights tracked: {_context.ChangeTracker.Entries<WinchWeight>().Count()}");*/
            
                await ImportShaftsAsync(workbook);
                
                /*Console.WriteLine(
                    $"Shafts tracked: {_context.ChangeTracker.Entries<WinchShaft>().Count()}");*/
                
                    await ImportChainsAsync(workbook);
                    
                    /*Console.WriteLine(
                        $"Chains tracked: {_context.ChangeTracker.Entries<WinchChain>().Count()}");*/
                    
                    /*var saved = await _context.SaveChangesAsync();*/

                    /*Console.WriteLine($"Weight/Shaft/Chain saved: {saved}");*/
                    
        await _context.SaveChangesAsync();
            await ImportDesignsAsync(workbook);
        
        Console.WriteLine("=== Winch Excel import finished ===");
    }

    private async Task ImportSeriesAsync(XLWorkbook workbook)
    {
        var worksheet = workbook.Worksheet("Series");
            var rows = worksheet.RowsUsed().Skip(1);
                var existingNames = await _context.WinchAnchorSeries
                    .Select(x => x.Name)
                        .ToListAsync();

        foreach (var row in rows)
        {
            /*Console.WriteLine(
                $"Row {row.RowNumber()} | " +
                $"A='{row.Cell(1).GetString()}' | " +
                $"B='{row.Cell(2).GetString()}' | " +
                $"C='{row.Cell(3).GetString()}'");*/
            
            var seriesName = row.Cell(1).GetString().Trim();
            Console.WriteLine($"SERIES: '{seriesName}'");
            
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

        /*Console.WriteLine(
            $"Series tracked: {_context.ChangeTracker.Entries<WinchAnchorSeries>().Count()}");*/

        /*var saved = await _context.SaveChangesAsync();
        Console.WriteLine($"Series saved: {saved}");*/
        
        await _context.SaveChangesAsync();
    }

    private async Task ImportWeightsAsync(XLWorkbook workbook)
    {
        var worksheet = workbook.Worksheet("Weight");

        var rows = worksheet.RowsUsed().Skip(1);

        var seriesDictionary = await _context.WinchAnchorSeries
            .ToDictionaryAsync(
                x => x.Name,
                x => x.Id);

        foreach (var row in rows)
        {
            var seriesName = row.Cell(1).GetString().Trim();
            
            /*Console.WriteLine($"WEIGHT: '{seriesName}'");*/
            
            if (string.IsNullOrWhiteSpace(seriesName))
                continue;

            var valueKg = row.Cell(2).GetValue<int>();
                var hour = row.Cell(3).GetValue<double>();

            if (!seriesDictionary.TryGetValue(
                    seriesName,
                    out var seriesId))
                continue;

            var weight = new WinchWeight
            {
                Name = seriesName,
                    ValueKg = valueKg,
                        Hour = hour,
                            WinchAnchorSeriesId = seriesId
            };

            _context.WinchWeights.Add(weight);
        }
    }
    
    private async Task ImportShaftsAsync(XLWorkbook workbook)
    {
        var worksheet = workbook.Worksheet("Shaft");

        var rows = worksheet.RowsUsed().Skip(1);

        var seriesDictionary = await _context.WinchAnchorSeries
            .ToDictionaryAsync(
                x => x.Name,
                x => x.Id);

        foreach (var row in rows)
        {
            var seriesName = row.Cell(1).GetString().Trim();
            
            /*Console.WriteLine($"SHAFT: '{seriesName}'");*/
            
            if (string.IsNullOrWhiteSpace(seriesName))
                continue;

            var valueMm = row.Cell(2).GetValue<int>();
                var hour = row.Cell(3).GetValue<double>();

            if (!seriesDictionary.TryGetValue(
                    seriesName,
                    out var seriesId))
            {
                continue;
            }

            var shaft = new WinchShaft
            {
                Name = seriesName,
                ValueMm = valueMm,
                Hour = hour,
                WinchAnchorSeriesId = seriesId
            };

            _context.WinchShafts.Add(shaft);
        }
    }
    
    private async Task ImportChainsAsync(XLWorkbook workbook)
    {
        var worksheet = workbook.Worksheet("Chain");

        var rows = worksheet.RowsUsed().Skip(1);

        var seriesDictionary = await _context.WinchAnchorSeries
            .ToDictionaryAsync(
                x => x.Name,
                x => x.Id);

        foreach (var row in rows)
        {
            var seriesName = row.Cell(1).GetString().Trim();
            
            /*Console.WriteLine($"CHAIN: '{seriesName}'");*/
            
            if (string.IsNullOrWhiteSpace(seriesName))
                continue;

            var valueKgMm = row.Cell(2).GetValue<int>();
            var hour = row.Cell(3).GetValue<double>();

            if (!seriesDictionary.TryGetValue(
                    seriesName,
                    out var seriesId))
            {
                continue;
            }

            var chain = new WinchChain
            {
                Name = seriesName,
                ValueKgMm = valueKgMm,
                Hour = hour,
                WinchAnchorSeriesId = seriesId
            };

            _context.WinchChains.Add(chain);
        }
    }

    private async Task ImportDesignsAsync(XLWorkbook workbook)
    {
        var worksheet = workbook.Worksheet("Design");
            var rows = worksheet.RowsUsed().Skip(1);

        foreach (var row in rows)
        {
            var seriesName = row.Cell(1).GetString().Trim();
            
            if (string.IsNullOrWhiteSpace(seriesName))
                continue;

            var valueKg = row.Cell(2).GetValue<int>();
                var valueMm = row.Cell(3).GetValue<int>();
                    var valueKgMm = row.Cell(4).GetValue<int>();

            var series = await _context.WinchAnchorSeries
                .FirstOrDefaultAsync(x => x.Name == seriesName);

            if (series == null)
                continue;

            var weight = await _context.WinchWeights
                .FirstOrDefaultAsync(x =>
                    x.WinchAnchorSeriesId == series.Id &&
                    x.ValueKg == valueKg);

            var shaft = await _context.WinchShafts
                .FirstOrDefaultAsync(x =>
                    x.WinchAnchorSeriesId == series.Id &&
                    x.ValueMm == valueMm);

            var chain = await _context.WinchChains
                .FirstOrDefaultAsync(x =>
                    x.WinchAnchorSeriesId == series.Id &&
                    x.ValueKgMm == valueKgMm);

            if (weight == null ||
                shaft == null ||
                chain == null)
            {
                continue;
            }

            var designExists = await _context.WinchAnchorDesigns
                .AnyAsync(x =>
                    x.WinchAnchorSeriesId == series.Id &&
                    x.WinchWeightId == weight.Id &&
                    x.WinchShaftId == shaft.Id &&
                    x.WinchChainId == chain.Id);

            if (designExists)
                continue;
            
            var design = new WinchAnchorDesign
            {
                Name = $"{seriesName}-{valueKg}-{valueMm}-{valueKgMm}",
                
                ValueKg = valueKg,
                ValueMm = valueMm,
                ValueKgMm = valueKgMm,

                Hour = Math.Round(
                    weight.Hour + shaft.Hour + chain.Hour,
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
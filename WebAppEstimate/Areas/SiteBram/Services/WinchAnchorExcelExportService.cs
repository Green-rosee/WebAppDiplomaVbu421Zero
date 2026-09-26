using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;

namespace WebAppEstimate.Areas.SiteBram.Services;

public class WinchAnchorExcelExportService:IWinchAnchorExcelExportService
{
    //private readonly AppDbContextWinchAnchor _context;
    
    private readonly IWebHostEnvironment _environment;

    public WinchAnchorExcelExportService(
        IWebHostEnvironment environment)
    {
        //_context = context;
        _environment = environment;
    }
    
    /*public async Task<string> SaveContractAsync()
    {
        var calculations = await _context
            .WinchAnchorCalculationHistories
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        var folder = Path.Combine(
            _environment.ContentRootPath,
            "ExcelFilesPageSave",
            "WinchAnchor");

        Directory.CreateDirectory(folder);

        var fileName =
            $"WinchAnchorContract_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        var filePath = Path.Combine(folder, fileName);

        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("Якорные лебёдки");

        worksheet.Cell("A1").Value = "Название";
        worksheet.Cell("B1").Value = "Серия";
        worksheet.Cell("C1").Value = "Масса, кг";
        worksheet.Cell("D1").Value = "Диаметр вала, мм";
        worksheet.Cell("E1").Value = "Калибр цепи, мм";
        worksheet.Cell("F1").Value = "Трудоёмкость, ч";
        worksheet.Cell("G1").Value = "Стоимость часа";
        worksheet.Cell("H1").Value = "Итоговая стоимость";

        var row = 2;

        foreach (var item in calculations)
        {
            worksheet.Cell(row, 1).Value = item.WinchName;
            worksheet.Cell(row, 2).Value = item.SeriesName;
            worksheet.Cell(row, 3).Value = item.SelectedValueKg;
            worksheet.Cell(row, 4).Value = item.SelectedValueMm;
            worksheet.Cell(row, 5).Value = item.SelectedValueKgMm;
            worksheet.Cell(row, 6).Value = item.Hour;
            worksheet.Cell(row, 7).Value = item.HourCost;
            worksheet.Cell(row, 8).Value = item.TotalCost;

            row++;
        }

        worksheet.Range("A1:H1").Style.Font.Bold = true;

        worksheet.Columns().AdjustToContents();

        worksheet.Column(6).Style.NumberFormat.Format = "0.00";
        worksheet.Column(7).Style.NumberFormat.Format = "0.00";
        worksheet.Column(8).Style.NumberFormat.Format = "0.00";

        workbook.SaveAs(filePath);

        return filePath;
    }*/

    public Task<string> SaveContractAsync(
        List<List<object?>> rows)
    {
       var folder = Path.Combine(
        _environment.ContentRootPath,
        "ExcelFilesPageSave",
        "WinchAnchor");

    Directory.CreateDirectory(folder);

    var fileName =
        $"WinchAnchorContract_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

    var filePath = Path.Combine(folder, fileName);

    using var workbook = new XLWorkbook();

    var worksheet =
        workbook.Worksheets.Add("Контракт");

    // --------------------------------------------------
    // Заголовок документа
    // --------------------------------------------------

    worksheet.Range("A1:I1").Merge();

    worksheet.Cell("A1").Value =
        "РАСЧЁТ СТОИМОСТИ ИЗГОТОВЛЕНИЯ ЯКОРНЫХ ЛЕБЁДОК";

    worksheet.Cell("A1").Style.Font.Bold = true;
    worksheet.Cell("A1").Style.Font.FontSize = 16;
    worksheet.Cell("A1").Style.Alignment.Horizontal =
        XLAlignmentHorizontalValues.Center;

    worksheet.Range("A2:I2").Merge();

    worksheet.Cell("A2").Value =
        $"Дата формирования: {DateTime.Now:dd.MM.yyyy}";

    worksheet.Cell("A2").Style.Alignment.Horizontal =
        XLAlignmentHorizontalValues.Right;


    // --------------------------------------------------
    // Заголовки таблицы
    // --------------------------------------------------

    var headerRow = 4;

    worksheet.Cell(headerRow, 1).Value = "№";
    worksheet.Cell(headerRow, 2).Value = "Наименование";
    worksheet.Cell(headerRow, 3).Value = "Серия";
    worksheet.Cell(headerRow, 4).Value = "Масса, кг";
    worksheet.Cell(headerRow, 5).Value = "Диаметр вала, мм";
    worksheet.Cell(headerRow, 6).Value = "Калибр цепи, мм";
    worksheet.Cell(headerRow, 7).Value = "Трудоёмкость, ч";
    worksheet.Cell(headerRow, 8).Value = "Стоимость часа";
    worksheet.Cell(headerRow, 9).Value = "Стоимость";


    var header = worksheet.Range(
        headerRow,
        1,
        headerRow,
        9);

    header.Style.Font.Bold = true;

    header.Style.Alignment.Horizontal =
        XLAlignmentHorizontalValues.Center;

    header.Style.Alignment.Vertical =
        XLAlignmentVerticalValues.Center;

    header.Style.Border.OutsideBorder =
        XLBorderStyleValues.Thin;

    header.Style.Border.InsideBorder =
        XLBorderStyleValues.Thin;


    // --------------------------------------------------
    // Данные из BlazorDatasheet
    // --------------------------------------------------

    var excelRow = headerRow + 1;

    // rows[0] — заголовки BlazorDatasheet,
    // поэтому начинаем с 1
    for (var i = 1; i < rows.Count; i++)
    {
        var row = rows[i];

        worksheet.Cell(excelRow, 1).Value = i;

        worksheet.Cell(excelRow, 2).Value =
            row[0]?.ToString() ?? "";

        worksheet.Cell(excelRow, 3).Value =
            row[1]?.ToString() ?? "";

        SetNumber(
            worksheet.Cell(excelRow, 4),
            row[2]);

        SetNumber(
            worksheet.Cell(excelRow, 5),
            row[3]);

        SetNumber(
            worksheet.Cell(excelRow, 6),
            row[4]);

        SetNumber(
            worksheet.Cell(excelRow, 7),
            row[5]);

        SetNumber(
            worksheet.Cell(excelRow, 8),
            row[6]);

        SetNumber(
            worksheet.Cell(excelRow, 9),
            row[7]);

        excelRow++;
    }


    // --------------------------------------------------
    // Границы таблицы
    // --------------------------------------------------

    var tableRange = worksheet.Range(
        headerRow,
        1,
        excelRow - 1,
        9);

    tableRange.Style.Border.OutsideBorder =
        XLBorderStyleValues.Thin;

    tableRange.Style.Border.InsideBorder =
        XLBorderStyleValues.Thin;


    // --------------------------------------------------
    // Итог
    // --------------------------------------------------

    worksheet.Range(excelRow + 1, 1, excelRow + 1, 8)
        .Merge();

    worksheet.Cell(excelRow + 1, 1).Value = "ИТОГО:";

    worksheet.Cell(excelRow + 1, 1)
        .Style.Font.Bold = true;

    worksheet.Cell(excelRow + 1, 1)
        .Style.Alignment.Horizontal =
        XLAlignmentHorizontalValues.Right;

    worksheet.Cell(excelRow + 1, 9)
        .FormulaA1 =
        $"SUM(I{headerRow + 1}:I{excelRow - 1})";

    worksheet.Cell(excelRow + 1, 9)
        .Style.Font.Bold = true;


    // --------------------------------------------------
    // Подписи
    // --------------------------------------------------

    worksheet.Cell(excelRow + 4, 2).Value =
        "Исполнитель:";

    worksheet.Cell(excelRow + 4, 3).Value =
        "________________________";

    worksheet.Cell(excelRow + 6, 2).Value =
        "Заказчик:";

    worksheet.Cell(excelRow + 6, 3).Value =
        "________________________";


    // --------------------------------------------------
    // Формат чисел
    // --------------------------------------------------

    worksheet.Column(7)
        .Style.NumberFormat.Format = "#,##0.00";

    worksheet.Column(8)
        .Style.NumberFormat.Format = "#,##0.00";

    worksheet.Column(9)
        .Style.NumberFormat.Format = "#,##0.00";


    // --------------------------------------------------
    // Размеры колонок
    // --------------------------------------------------

    worksheet.Column(1).Width = 6;
    worksheet.Column(2).Width = 25;
    worksheet.Column(3).Width = 20;
    worksheet.Column(4).Width = 14;
    worksheet.Column(5).Width = 18;
    worksheet.Column(6).Width = 18;
    worksheet.Column(7).Width = 18;
    worksheet.Column(8).Width = 18;
    worksheet.Column(9).Width = 20;


    // --------------------------------------------------
    // Печать
    // --------------------------------------------------

    worksheet.PageSetup.PageOrientation =
        XLPageOrientation.Landscape;

    worksheet.PageSetup.FitToPages(1, 0);

    workbook.SaveAs(filePath);

    return Task.FromResult(filePath);
    }
    
    private static void SetNumber(
        IXLCell cell,
        object? value)
    {
        if (value == null)
            return;

        if (double.TryParse(
                value.ToString(),
                out var number))
        {
            cell.Value = number;
        }
        else
        {
            cell.Value = value.ToString();
        }
    }
}


using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;

namespace WebAppEstimate.Areas.SiteBram.Services;


/// <summary>
/// Сервис формирования Excel-документа
/// с результатами расчёта якорных лебёдок.
/// </summary>
public class WinchAnchorExcelExportService
    : IWinchAnchorExcelExportService
{
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Создаёт сервис экспорта Excel-файлов.
    /// </summary>
    /// <param name="environment">
    /// Информация о текущем окружении приложения
    /// и корневом каталоге проекта.
    /// </param>
    public WinchAnchorExcelExportService(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Формирует Excel-документ на основе данных,
    /// полученных из таблицы BlazorDatasheet,
    /// и сохраняет его в файловой системе.
    /// </summary>
    /// <param name="rows">
    /// Строки таблицы с результатами расчётов.
    /// Первая строка содержит заголовки таблицы.
    /// </param>
    /// <returns>
    /// Полный путь к созданному Excel-файлу.
    /// </returns>
    public Task<string> SaveContractAsync(
        List<List<object?>> rows)
    {
        // Определяем каталог для сохранения сформированных документов.
        var folder = Path.Combine(
            _environment.ContentRootPath,
            "ExcelFilesPageSave",
            "WinchAnchor");

        Directory.CreateDirectory(folder);

        // Формируем уникальное имя файла
        // на основе текущей даты и времени.
        var fileName =
            $"WinchAnchorContract_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        var filePath =
            Path.Combine(folder, fileName);

        using var workbook =
            new XLWorkbook();

        var worksheet =
            workbook.Worksheets.Add("Контракт");

        // Формируем заголовок документа.
        worksheet.Range("A1:I1").Merge();

        worksheet.Cell("A1").Value =
            "РАСЧЁТ СТОИМОСТИ ИЗГОТОВЛЕНИЯ ЯКОРНЫХ ЛЕБЁДОК";

        worksheet.Cell("A1")
            .Style.Font.Bold = true;

        worksheet.Cell("A1")
            .Style.Font.FontSize = 16;

        worksheet.Cell("A1")
            .Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        // Добавляем дату формирования документа.
        worksheet.Range("A2:I2").Merge();

        worksheet.Cell("A2").Value =
            $"Дата формирования: {DateTime.Now:dd.MM.yyyy}";

        worksheet.Cell("A2")
            .Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Right;

        // Формируем заголовки основной таблицы.
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

        var header =
            worksheet.Range(
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

        // Первая строка rows содержит заголовки BlazorDatasheet,
        // поэтому экспорт данных начинается со второй строки.
        var excelRow = headerRow + 1;

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

        // Добавляем границы ко всей таблице.
        var tableRange =
            worksheet.Range(
                headerRow,
                1,
                excelRow - 1,
                9);

        tableRange.Style.Border.OutsideBorder =
            XLBorderStyleValues.Thin;

        tableRange.Style.Border.InsideBorder =
            XLBorderStyleValues.Thin;

        // Добавляем итоговую строку
        // с суммой стоимости всех позиций.
        worksheet.Range(
                excelRow + 1,
                1,
                excelRow + 1,
                8)
            .Merge();

        worksheet.Cell(
                excelRow + 1,
                1)
            .Value = "ИТОГО:";

        worksheet.Cell(
                excelRow + 1,
                1)
            .Style.Font.Bold = true;

        worksheet.Cell(
                excelRow + 1,
                1)
            .Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Right;

        worksheet.Cell(
                excelRow + 1,
                9)
            .FormulaA1 =
            $"SUM(I{headerRow + 1}:I{excelRow - 1})";

        worksheet.Cell(
                excelRow + 1,
                9)
            .Style.Font.Bold = true;

        // Добавляем строки для подписей исполнителя и заказчика.
        worksheet.Cell(
                excelRow + 4,
                2)
            .Value = "Исполнитель:";

        worksheet.Cell(
                excelRow + 4,
                3)
            .Value =
            "________________________";

        worksheet.Cell(
                excelRow + 6,
                2)
            .Value = "Заказчик:";

        worksheet.Cell(
                excelRow + 6,
                3)
            .Value =
            "________________________";

        // Форматируем числовые колонки.
        worksheet.Column(7)
            .Style.NumberFormat.Format =
            "#,##0.00";

        worksheet.Column(8)
            .Style.NumberFormat.Format =
            "#,##0.00";

        worksheet.Column(9)
            .Style.NumberFormat.Format =
            "#,##0.00";

        // Настраиваем ширину колонок.
        worksheet.Column(1).Width = 6;
        worksheet.Column(2).Width = 25;
        worksheet.Column(3).Width = 20;
        worksheet.Column(4).Width = 14;
        worksheet.Column(5).Width = 18;
        worksheet.Column(6).Width = 18;
        worksheet.Column(7).Width = 18;
        worksheet.Column(8).Width = 18;
        worksheet.Column(9).Width = 20;

        // Настраиваем лист для печати.
        worksheet.PageSetup.PageOrientation =
            XLPageOrientation.Landscape;

        worksheet.PageSetup.FitToPages(
            1,
            0);

        // Сохраняем сформированный Excel-файл.
        workbook.SaveAs(filePath);

        return Task.FromResult(filePath);
    }

    /// <summary>
    /// Записывает значение в Excel-ячейку как число,
    /// если значение можно преобразовать в double.
    /// В противном случае сохраняет его как текст.
    /// </summary>
    /// <param name="cell">
    /// Excel-ячейка, в которую записывается значение.
    /// </param>
    /// <param name="value">
    /// Значение из BlazorDatasheet.
    /// </param>
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
            cell.Value =
                value.ToString();
        }
    }
}
























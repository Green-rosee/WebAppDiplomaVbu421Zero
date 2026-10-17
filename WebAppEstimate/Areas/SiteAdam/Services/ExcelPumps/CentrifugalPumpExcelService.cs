using ClosedXML.Excel;
using WebAppEstimate.Areas.SiteAdam.Data.Entity.Pumps.CentrifugalPumpsCalculationHistory;

namespace WebAppEstimate.Areas.SiteAdam.Services.ExcelPumps;




/// <summary>
/// Сервис формирования Excel-файла
/// с историей расчётов центробежных насосов.
/// </summary>
public class CentrifugalPumpExcelService
    : ICentrifugalPumpExcelService
{
    /// <summary>
    /// Создаёт Excel-документ на основе истории расчётов
    /// центробежных насосов.
    /// </summary>
    /// <param name="histories">
    /// Список сохранённых расчётов центробежных насосов.
    /// </param>
    /// <returns>
    /// Excel-файл в виде массива байтов.
    /// </returns>
    public byte[] CreateExcel(
        List<PumpCentrifugalHistory> histories)
    {
        // Создаём новую Excel-книгу в памяти.
        using var workbook =
            new XLWorkbook();

        // Добавляем рабочий лист для данных насосов.
        var worksheet =
            workbook.Worksheets.Add("Насосы");

        // Формируем заголовки таблицы.
        worksheet.Cell(1, 1).Value = "Наименование";
        worksheet.Cell(1, 2).Value = "Количество";
        worksheet.Cell(1, 3).Value = "Стоимость часа";
        worksheet.Cell(1, 4).Value = "Трудоемкость";
        worksheet.Cell(1, 5).Value = "Общая стоимость";
        worksheet.Cell(1, 6).Value = "Дата";

        // Данные начинаются со второй строки,
        // так как первая используется для заголовков.
        var row = 2;

        foreach (var item in histories)
        {
            worksheet.Cell(row, 1).Value =
                item.Name;

            worksheet.Cell(row, 2).Value =
                item.Number;

            worksheet.Cell(row, 3).Value =
                item.CostHour;

            worksheet.Cell(row, 4).Value =
                item.TotalSumHour;

            worksheet.Cell(row, 5).Value =
                item.TotalSumCostHour;

            worksheet.Cell(row, 6).Value =
                item.CreatedAt;

            row++;
        }

        // Сохраняем Excel-книгу во временный поток памяти.
        using var stream =
            new MemoryStream();

        workbook.SaveAs(stream);

        // Возвращаем готовый XLSX-файл
        // как массив байтов для дальнейшей отправки или скачивания.
        return stream.ToArray();
    }
}












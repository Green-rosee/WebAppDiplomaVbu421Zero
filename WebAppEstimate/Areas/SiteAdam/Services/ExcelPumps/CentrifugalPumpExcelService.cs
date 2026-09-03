using ClosedXML.Excel;
using WebAppEstimate.Areas.SiteAdam.Data.Entity.Pumps.CentrifugalPumpsCalculationHistory;

namespace WebAppEstimate.Areas.SiteAdam.Services.ExcelPumps;

public class CentrifugalPumpExcelService:ICentrifugalPumpExcelService
{
    public byte[] CreateExcel(List<PumpCentrifugalHistory> histories)
    {
        using var workbook = new XLWorkbook();

        var worksheet =
            workbook.Worksheets.Add("Насосы");

        worksheet.Cell(1, 1).Value = "Наименование";
        worksheet.Cell(1, 2).Value = "Количество";
        worksheet.Cell(1, 3).Value = "Стоимость часа";
        worksheet.Cell(1, 4).Value = "Трудоемкость";
        worksheet.Cell(1, 5).Value = "Общая стоимость";
        worksheet.Cell(1, 6).Value = "Дата";

        var row = 2;

        foreach (var item in histories)
        {
            worksheet.Cell(row, 1).Value = item.Name;
            worksheet.Cell(row, 2).Value = item.Number;
            worksheet.Cell(row, 3).Value = item.CostHour;
            worksheet.Cell(row, 4).Value = item.TotalSumHour;
            worksheet.Cell(row, 5).Value = item.TotalSumCostHour;
            worksheet.Cell(row, 6).Value = item.CreatedAt;

            row++;
        }

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return stream.ToArray();
    }
}
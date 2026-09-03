using WebAppEstimate.Areas.SiteAdam.Data.Entity.Pumps.CentrifugalPumpsCalculationHistory;

namespace WebAppEstimate.Areas.SiteAdam.Services.ExcelPumps;

public interface ICentrifugalPumpExcelService
{
    byte[] CreateExcel(List<PumpCentrifugalHistory> histories);
}
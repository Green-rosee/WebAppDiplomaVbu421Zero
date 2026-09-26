namespace WebAppEstimate.Areas.SiteBram.Services;

public interface IWinchAnchorExcelExportService
{
    /*Task<string> SaveContractAsync();*/
    
    Task<string> SaveContractAsync(
        List<List<object?>> rows);
}
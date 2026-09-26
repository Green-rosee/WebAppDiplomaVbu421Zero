using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

namespace WebAppEstimate.Areas.SiteBram.Services;

public interface IWinchAnchorHistoryService
{
    Task<Guid> SaveAsync(WinchAnchorCalculationHistory history);

    Task<WinchAnchorCalculationHistory?> GetByIdAsync(Guid id);
    
    Task<List<WinchAnchorCalculationHistory>> GetAllAsync();
}
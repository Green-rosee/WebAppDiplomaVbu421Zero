using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

namespace WebAppEstimate.Areas.SiteBram.Services;

public class WinchAnchorHistoryService:IWinchAnchorHistoryService
{
    private readonly AppDbContextWinchAnchor _context;

    public WinchAnchorHistoryService(
        AppDbContextWinchAnchor context)
    {
        _context = context;
    }

    public async Task<Guid> SaveAsync(WinchAnchorCalculationHistory history)
    {
        _context.WinchAnchorCalculationHistories.Add(history);

        await _context.SaveChangesAsync();

        return history.Id;
    }

    public async Task<WinchAnchorCalculationHistory?> GetByIdAsync(Guid id)
    {
        return await _context.WinchAnchorCalculationHistories
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<WinchAnchorCalculationHistory>> GetAllAsync()
    {
        return await _context.WinchAnchorCalculationHistories
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }
}
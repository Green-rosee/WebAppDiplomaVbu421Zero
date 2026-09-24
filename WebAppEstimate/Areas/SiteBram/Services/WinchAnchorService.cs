using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

namespace WebAppEstimate.Areas.SiteBram.Services;

public class WinchAnchorService : IWinchAnchorService
{
    private readonly AppDbContextWinchAnchor _context;

    public WinchAnchorService(
        AppDbContextWinchAnchor context)
    {
        _context = context;
    }

    public async Task<List<WinchAnchorSeries>> GetSeriesAsync()
    {
        return await _context.WinchAnchorSeries
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<WinchAnchorDesign?> FindDesignAsync(
        Guid seriesId,
        int valueKg,
        int valueMm,
        int valueKgMm)
    {
        return await _context.WinchAnchorDesigns
            .FirstOrDefaultAsync(x =>
                x.WinchAnchorSeriesId == seriesId &&
                x.ValueKg == valueKg &&
                x.ValueMm == valueMm &&
                x.ValueKgMm == valueKgMm);
    }
}
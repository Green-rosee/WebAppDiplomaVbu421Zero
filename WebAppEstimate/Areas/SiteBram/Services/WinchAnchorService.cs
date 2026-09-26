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
        var designs = await _context.WinchAnchorDesigns
            .Where(x => x.WinchAnchorSeriesId == seriesId)
            .ToListAsync();

        if (designs.Count == 0)
            return null;

        var closestKg = designs
            .OrderBy(x => Math.Abs(x.ValueKg - valueKg))
            .First()
            .ValueKg;

        var closestByKg = designs
            .Where(x => x.ValueKg == closestKg)
            .ToList();

        var closestMm = closestByKg
            .OrderBy(x => Math.Abs(x.ValueMm - valueMm))
            .First()
            .ValueMm;

        var closestByKgAndMm = closestByKg
            .Where(x => x.ValueMm == closestMm)
            .ToList();

        return closestByKgAndMm
            .OrderBy(x => Math.Abs(x.ValueKgMm - valueKgMm))
            .FirstOrDefault();
    }
}
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

namespace WebAppEstimate.Areas.SiteBram.Services;

public interface IWinchAnchorService
{
    Task<List<WinchAnchorSeries>> GetSeriesAsync();

    Task<WinchAnchorDesign?> FindDesignAsync(
        Guid seriesId,
        int valueKg,
        int valueMm,
        int valueKgMm);
}
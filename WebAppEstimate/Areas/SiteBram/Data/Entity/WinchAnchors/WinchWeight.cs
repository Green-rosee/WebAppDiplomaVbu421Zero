namespace WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

public class WinchWeight:AWinchBase
{
    public int WinchAnchorSeriesId { get; set; }
    public WinchAnchorSeries WinchAnchorSeries { get; set; } = null!;
}
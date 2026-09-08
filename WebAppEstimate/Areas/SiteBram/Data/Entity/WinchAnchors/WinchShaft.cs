namespace WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

public class WinchShaft:AWinchBase
{
    public int WinchAnchorSeriesId { get; set; }
    public WinchAnchorSeries WinchAnchorSeries { get; set; } = null!;
}
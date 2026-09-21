namespace WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

public class WinchShaft:AWinchBase
{
    public Guid WinchAnchorSeriesId { get; set; }
    public WinchAnchorSeries WinchAnchorSeries { get; set; } = null!;
}
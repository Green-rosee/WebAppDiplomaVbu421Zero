namespace WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

public class WinchChain:AWinchBase
{
    public Guid WinchAnchorSeriesId { get; set; }
    public WinchAnchorSeries WinchAnchorSeries { get; set; } = null!;
}
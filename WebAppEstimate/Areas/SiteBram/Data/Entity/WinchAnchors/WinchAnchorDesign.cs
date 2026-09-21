namespace WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

public class WinchAnchorDesign:AWinchBase
{
    // 1. Связь с серией
    public Guid WinchAnchorSeriesId { get; set; }
    public WinchAnchorSeries WinchAnchorSeries { get; set; } = null!;

    // 2. Связь с конкретными значениями
    public Guid WinchWeightId { get; set; }
    public WinchWeight WinchWeight { get; set; } = null!;

    public Guid WinchShaftId { get; set; }
    public WinchShaft WinchShaft { get; set; } = null!;

    public Guid WinchChainId { get; set; }
    public WinchChain WinchChain { get; set; } = null!;
    
    
    
}


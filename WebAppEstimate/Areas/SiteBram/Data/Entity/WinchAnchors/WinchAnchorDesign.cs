namespace WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

public class WinchAnchorDesign:AWinchBase
{
    // 1. Связь с серией
    public int WinchAnchorSeriesId { get; set; }
    public WinchAnchorSeries WinchAnchorSeries { get; set; } = null!;

    // 2. Связь с конкретными значениями
    public int WinchWeightId { get; set; }
    public WinchWeight WinchWeight { get; set; } = null!;

    public int WinchShaftId { get; set; }
    public WinchShaft WinchShaft { get; set; } = null!;

    public int WinchChainId { get; set; }
    public WinchChain WinchChain { get; set; } = null!;
    
    
    
}

/*public int Id { get; set; }

// 1. Связь с Серией
public int PumpSeriesId { get; set; }
public PumpSeries PumpSeries { get; set; } = null!;

// 2. Связь с конкретным Колесом (размером и его часами)
public int PumpImpellerId { get; set; }
public PumpImpeller PumpImpeller { get; set; } = null!;

// 3. Связь с конкретным Весом (значением и его часами)
public int PumpWeightId { get; set; }
public PumpWeight PumpWeight { get; set; } = null!;

// 4. Связь с конкретным Объемом (значением и его часами)
public int PumpVolumeId { get; set; }
public PumpVolume PumpVolume { get; set; } = null!;*/


//===========================
// public int WinchAnchorSeriesId { get; set; }
// public WinchAnchorSeries WinchAnchorSeries { get; set; } = null!;
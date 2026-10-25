using WebAppEstimate.Areas.SiteAdam.Models;

namespace WebAppEstimate.Areas.SiteAdam.Data.Entity.CentrifugalPumps;

public class PumpVolume
{
    public int Id { get; set; }
    public int PumpSeriesId { get; set; }

    public PumpSeries PumpSeries { get; set; } = null!;
    
    public int PumpNormativeSetId { get; set; }
    public PumpNormativeSet PumpNormativeSet { get; set; } = null!;

    public int Volume { get; set; }
    public double Hour { get; set; }
}
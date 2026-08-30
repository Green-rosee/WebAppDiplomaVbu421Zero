namespace WebAppEstimate.Areas.SiteAdam.Data.Entity.CentrifugalPumps;

public class PumpImpeller
{
    public int Id { get; set; }
    public int PumpSeriesId { get; set; }
    public PumpSeries PumpSeries { get; set; } = null!;

    public int Impeller { get; set; }
    public double Hour { get; set; }
}
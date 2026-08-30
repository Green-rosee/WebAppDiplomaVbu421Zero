namespace WebAppEstimate.Areas.SiteAdam.Data.Entity.CentrifugalPumps;

public class PumpSeries
{
    public int Id { get; set; }

    public string Name { get; set; }

    //---
    public ICollection<PumpImpeller> Impeller { get; set; } = new List<PumpImpeller>();
    public ICollection<PumpWeight> Weight { get; set; } = new List<PumpWeight>();
    public ICollection<PumpVolume> Volume { get; set; } = new List<PumpVolume>();
}
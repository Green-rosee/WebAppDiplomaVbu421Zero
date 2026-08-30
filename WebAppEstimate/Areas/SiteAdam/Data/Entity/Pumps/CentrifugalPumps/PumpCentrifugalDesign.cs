namespace WebAppEstimate.Areas.SiteAdam.Data.Entity.CentrifugalPumps;

public class PumpCentrifugalDesign
{
    public int Id { get; set; }

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
    public PumpVolume PumpVolume { get; set; } = null!;
}
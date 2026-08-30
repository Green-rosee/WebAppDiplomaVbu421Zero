using WebAppEstimate.Areas.SiteAdam.Data.Entity.CentrifugalPumps;

namespace WebAppEstimate.Areas.SiteAdam.Data.Entity.Pumps.CentrifugalPumpsCalculationHistory;

public class PumpCentrifugalHistory
{
    public int Id { get; set; }

    // Введенные пользователем данные
    public string Name { get; set; } = string.Empty;
    public int SelectedSeriesId { get; set; }
    public int Impeller { get; set; }
    public int Weight { get; set; }
    public int Volume { get; set; }
    public int Number { get; set; }

    public decimal CostHour { get; set; }

    public double TotalSumHour { get; set; }
    public decimal TotalSumCostHour { get; set; }

    // Дата и время проведения расчета
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационное свойство для связи с таблицей серий (чтобы вытянуть текстовое имя серии в таблицу)
    public PumpSeries? PumpSeries { get; set; }
}
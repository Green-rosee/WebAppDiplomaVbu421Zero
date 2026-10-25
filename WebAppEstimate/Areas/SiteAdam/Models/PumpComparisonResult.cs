namespace WebAppEstimate.Areas.SiteAdam.Models;

public class PumpComparisonResult
{
    public int Id { get; set; }

    public string ScenarioName { get; set; } = "";

    public string PumpName { get; set; } = "";

    public double Impeller { get; set; }
    public double Weight { get; set; }
    public double Volume { get; set; }

    public int Number { get; set; }

    public double TotalSumHour { get; set; }
    public decimal TotalSumCostHour { get; set; }

    public DateTime CreatedAt { get; set; }
    public string DataSetName { get; set; } = "";
}
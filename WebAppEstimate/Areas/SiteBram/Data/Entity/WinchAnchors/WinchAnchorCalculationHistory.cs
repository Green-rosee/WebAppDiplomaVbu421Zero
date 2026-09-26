namespace WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

public class WinchAnchorCalculationHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string WinchName { get; set; } = string.Empty;

    public Guid WinchAnchorSeriesId { get; set; }

    public string SeriesName { get; set; } = string.Empty;

    public int InputValueKg { get; set; }

    public int InputValueMm { get; set; }

    public int InputValueKgMm { get; set; }

    public int SelectedValueKg { get; set; }

    public int SelectedValueMm { get; set; }

    public int SelectedValueKgMm { get; set; }

    public double Hour { get; set; }

    public decimal HourCost { get; set; }

    public decimal TotalCost { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
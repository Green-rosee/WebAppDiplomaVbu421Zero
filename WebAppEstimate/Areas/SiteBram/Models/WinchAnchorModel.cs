namespace WebAppEstimate.Areas.SiteBram.Models;

public class WinchAnchorModel
{
    public Guid? SeriesId { get; set; }

    public int ValueKg { get; set; }

    public int ValueMm { get; set; }

    public int ValueKgMm { get; set; }
    
    public decimal HourCost { get; set; }

    public int Quantity { get; set; }
}
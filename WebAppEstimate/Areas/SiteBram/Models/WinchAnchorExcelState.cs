namespace WebAppEstimate.Areas.SiteBram.Models;

public class WinchAnchorExcelState
{
    public string WinchName { get; set; } = string.Empty;

    public string SeriesName { get; set; } = string.Empty;

    public int ValueKg { get; set; }

    public int ValueMm { get; set; }

    public int ValueKgMm { get; set; }

    public double Hour { get; set; }

    public decimal HourCost { get; set; }

    public decimal TotalCost { get; set; }

    public void Clear()
    {
        WinchName = string.Empty;
        SeriesName = string.Empty;

        ValueKg = 0;
        ValueMm = 0;
        ValueKgMm = 0;

        Hour = 0;
        HourCost = 0;
        TotalCost = 0;
    }
}
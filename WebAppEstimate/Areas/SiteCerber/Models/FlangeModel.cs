namespace WebAppEstimate.Areas.SiteCerber.Models;

public class FlangeModel
{
    public string Name { get; set; } = "Flange";

    public double OuterDiameter { get; set; }
    public double InnerDiameter { get; set; }
    public double Thickness { get; set; }

    public int HoleCount { get; set; }
    public double HoleDiameter { get; set; }

    public double BoltCircleDiameter { get; set; }
}

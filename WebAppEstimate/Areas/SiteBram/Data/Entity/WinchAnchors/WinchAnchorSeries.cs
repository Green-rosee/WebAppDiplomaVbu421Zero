namespace WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

public class WinchAnchorSeries : AWinchBase
{
    
public ICollection < WinchChain > Chains { get; set; } = new List<WinchChain>();
public ICollection < WinchShaft > Shafts { get; set; } = new List<WinchShaft>();
public ICollection < WinchWeight > Weights { get; set; } = new List<WinchWeight>();


}
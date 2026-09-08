namespace WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

public abstract class AWinchBase
{
    public virtual int Id { get; set; }

    public virtual string Name { get; set; }=string.Empty;

    public virtual int ValueKg { get; set; }
    public virtual int ValueMm { get; set; }
    public virtual int ValueKgMm { get; set; }
    public virtual double Hour { get; set; }
}
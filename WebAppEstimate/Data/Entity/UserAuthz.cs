using WebAppEstimate.Areas.SiteAdam.Data.Entity;

namespace WebAppEstimate.Data.Entity;

public class UserAuthz
{
    public int Id { get; set; }
    public string? Login { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
}
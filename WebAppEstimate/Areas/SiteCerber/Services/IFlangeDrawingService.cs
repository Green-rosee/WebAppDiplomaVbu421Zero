using WebAppEstimate.Areas.SiteCerber.Models;

namespace WebAppEstimate.Areas.SiteCerber.Services;

public interface IFlangeDrawingService
{
    Task<string> GenerateSvgAsync(FlangeModel model);
}
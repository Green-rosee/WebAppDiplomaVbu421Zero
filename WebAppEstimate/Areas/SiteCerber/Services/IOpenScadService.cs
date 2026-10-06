using WebAppEstimate.Areas.SiteCerber.Models;

namespace WebAppEstimate.Areas.SiteCerber.Services;

public interface IOpenScadService
{
    Task<string> GenerateFlangeAsync(FlangeModel model);
}
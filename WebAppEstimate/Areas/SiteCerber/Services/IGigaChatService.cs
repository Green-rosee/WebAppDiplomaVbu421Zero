using WebAppEstimate.Areas.SiteCerber.Models;

namespace WebAppEstimate.Areas.SiteCerber.Services;

public interface IGigaChatService
{
    Task<FlangeModel?> ParseFlangeAsync(string prompt);
}
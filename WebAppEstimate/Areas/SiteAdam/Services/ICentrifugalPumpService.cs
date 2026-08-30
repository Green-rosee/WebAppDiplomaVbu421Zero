using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppEstimate.Areas.SiteAdam.Models;

namespace WebAppEstimate.Areas.SiteAdam.Services;

public interface ICentrifugalPumpService
{
    // Метод для получения списка серий для выпадающего списка
    Task<List<SelectListItem>> GetSeriesOptionsAsync();

    // Метод для выполнения расчета близких значений и суммирования часов
    Task<CentrifugalPumpModel> CalculateClosestHoursAsync(CentrifugalPumpModel model);
}
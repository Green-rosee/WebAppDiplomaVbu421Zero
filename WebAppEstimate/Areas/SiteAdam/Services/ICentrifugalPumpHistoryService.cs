using WebAppEstimate.Areas.SiteAdam.Data.Entity.Pumps.CentrifugalPumpsCalculationHistory;
using WebAppEstimate.Areas.SiteAdam.Models;

namespace WebAppEstimate.Areas.SiteAdam.Services;

public interface ICentrifugalPumpHistoryService
{
    //--Добавлено
    Task SaveAsync(CentrifugalPumpModel model);


    Task<List<PumpCentrifugalHistory>> GetAllAsync();

    //------------------------------
    Task DeleteAsync(int id);
    Task DeleteAllAsync();
}
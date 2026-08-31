using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;
using WebAppEstimate.Areas.SiteAdam.Data.Entity.Pumps.CentrifugalPumpsCalculationHistory;
using WebAppEstimate.Areas.SiteAdam.Models;

namespace WebAppEstimate.Areas.SiteAdam.Services;

public class CentrifugalPumpHistoryService : ICentrifugalPumpHistoryService
{
    private readonly AppDbContextCentrifugalPump _dbcontext;

    public CentrifugalPumpHistoryService(AppDbContextCentrifugalPump dbcontext)
    {
        _dbcontext = dbcontext;
    }


    public async Task SaveAsync(CentrifugalPumpModel model)
    {
        var history = new PumpCentrifugalHistory
        {
            Name = model.Name,
            SelectedSeriesId = model.SelectedSeriesId,

            Impeller = model.Impeller,
            Weight = model.Weight,
            Volume = model.Volume,

            Number = model.Number,

            CostHour = model.CostHour,

            TotalSumHour = model.TotalSumHour,
            TotalSumCostHour = model.TotalSumCostHour,

            CreatedAt = DateTime.UtcNow
        };

        _dbcontext.PumpCentrifugalHistories.Add(history);

        await _dbcontext.SaveChangesAsync();
    }


    public async Task<List<PumpCentrifugalHistory>> GetAllAsync()
    {
        return await _dbcontext.PumpCentrifugalHistories
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _dbcontext
            .PumpCentrifugalHistories
            .FindAsync(id);

        if (item == null)
            return;

        _dbcontext.PumpCentrifugalHistories.Remove(item);

        await _dbcontext.SaveChangesAsync();
    }

    public async Task DeleteAllAsync()
    {
        var items = await _dbcontext.PumpCentrifugalHistories.ToListAsync();

        _dbcontext.PumpCentrifugalHistories.RemoveRange(items);

        await _dbcontext.SaveChangesAsync();
    }
}
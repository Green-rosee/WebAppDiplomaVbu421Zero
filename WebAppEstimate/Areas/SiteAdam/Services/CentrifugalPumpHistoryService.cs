using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;
using WebAppEstimate.Areas.SiteAdam.Data.Entity.Pumps.CentrifugalPumpsCalculationHistory;
using WebAppEstimate.Areas.SiteAdam.Models;

namespace WebAppEstimate.Areas.SiteAdam.Services;






/// <summary>
/// Сервис работы с историей расчётов центробежных насосов.
/// Отвечает за сохранение, получение и удаление
/// результатов расчётов из базы данных.
/// </summary>
public class CentrifugalPumpHistoryService
    : ICentrifugalPumpHistoryService
{
    private readonly AppDbContextCentrifugalPump _dbcontext;

    /// <summary>
    /// Создаёт сервис истории расчётов центробежных насосов.
    /// </summary>
    /// <param name="dbcontext">
    /// Контекст базы данных SiteAdam.
    /// </param>
    public CentrifugalPumpHistoryService(
        AppDbContextCentrifugalPump dbcontext)
    {
        _dbcontext = dbcontext;
    }

    /// <summary>
    /// Сохраняет результат расчёта центробежного насоса
    /// в историю базы данных.
    /// </summary>
    /// <param name="model">
    /// Модель с исходными параметрами
    /// и результатами выполненного расчёта.
    /// </param>
    public async Task SaveAsync(
        CentrifugalPumpModel model)
    {
        // Формируем отдельную сущность истории
        // на основе текущего состояния расчётной модели.
        var history =
            new PumpCentrifugalHistory
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

                // Сохраняем время создания записи в UTC.
                CreatedAt = DateTime.UtcNow
            };

        _dbcontext.PumpCentrifugalHistories
            .Add(history);

        await _dbcontext.SaveChangesAsync();
    }

    /// <summary>
    /// Получает всю историю расчётов центробежных насосов.
    /// </summary>
    /// <returns>
    /// Список расчётов, отсортированный
    /// от самых новых записей к более старым.
    /// </returns>
    public async Task<List<PumpCentrifugalHistory>> GetAllAsync()
    {
        return await _dbcontext
            .PumpCentrifugalHistories
            .OrderByDescending(
                h => h.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Удаляет одну запись истории по её идентификатору.
    /// </summary>
    /// <param name="id">
    /// Идентификатор удаляемой записи.
    /// </param>
    public async Task DeleteAsync(int id)
    {
        // Ищем запись по первичному ключу.
        var item =
            await _dbcontext
                .PumpCentrifugalHistories
                .FindAsync(id);

        // Если запись не найдена,
        // дополнительная обработка не требуется.
        if (item == null)
            return;

        _dbcontext
            .PumpCentrifugalHistories
            .Remove(item);

        await _dbcontext.SaveChangesAsync();
    }

    /// <summary>
    /// Удаляет все записи истории расчётов
    /// центробежных насосов.
    /// </summary>
    public async Task DeleteAllAsync()
    {
        // Загружаем все записи истории.
        var items =
            await _dbcontext
                .PumpCentrifugalHistories
                .ToListAsync();

        // Помечаем все записи на удаление.
        _dbcontext
            .PumpCentrifugalHistories
            .RemoveRange(items);

        await _dbcontext.SaveChangesAsync();
    }
}








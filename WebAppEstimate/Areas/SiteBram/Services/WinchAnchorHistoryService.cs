using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

namespace WebAppEstimate.Areas.SiteBram.Services;


/// <summary>
/// Сервис работы с историей расчётов якорных лебёдок.
/// Отвечает за сохранение и получение результатов расчётов из базы данных.
/// </summary>
public class WinchAnchorHistoryService : IWinchAnchorHistoryService
{
    private readonly AppDbContextWinchAnchor _context;

    /// <summary>
    /// Создаёт сервис истории расчётов якорных лебёдок.
    /// </summary>
    /// <param name="context">
    /// Контекст базы данных SiteBram.
    /// </param>
    public WinchAnchorHistoryService(
        AppDbContextWinchAnchor context)
    {
        _context = context;
    }

    /// <summary>
    /// Сохраняет результат расчёта якорной лебёдки в базе данных.
    /// </summary>
    /// <param name="history">
    /// Объект истории расчёта, содержащий рассчитанные параметры.
    /// </param>
    /// <returns>
    /// Идентификатор сохранённой записи.
    /// </returns>
    public async Task<Guid> SaveAsync(
        WinchAnchorCalculationHistory history)
    {
        // Добавляем новую запись истории в контекст EF Core.
        _context.WinchAnchorCalculationHistories
            .Add(history);

        // Сохраняем изменения в базе данных.
        await _context.SaveChangesAsync();

        return history.Id;
    }

    /// <summary>
    /// Получает одну запись истории расчёта по её идентификатору.
    /// </summary>
    /// <param name="id">
    /// Уникальный идентификатор записи истории.
    /// </param>
    /// <returns>
    /// Найденная запись либо null, если запись отсутствует.
    /// </returns>
    public async Task<WinchAnchorCalculationHistory?> GetByIdAsync(
        Guid id)
    {
        return await _context.WinchAnchorCalculationHistories
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Получает всю историю расчётов якорных лебёдок.
    /// </summary>
    /// <returns>
    /// Список записей истории, отсортированный по времени создания.
    /// </returns>
    public async Task<List<WinchAnchorCalculationHistory>> GetAllAsync()
    {
        return await _context.WinchAnchorCalculationHistories
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }
}












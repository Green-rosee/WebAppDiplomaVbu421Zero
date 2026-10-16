using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;
using WebAppEstimate.Areas.SiteBram.Services;

namespace WebAppEstimate.Areas.SiteBram.Pipeline;


/// <summary>
/// Выполняет первоначальную инициализацию базы данных SiteBram.
/// При необходимости пересоздаёт базу и загружает исходные данные
/// якорных лебёдок из Excel.
/// </summary>
public static class DatabaseInitWinchAnchor
{
    /// <summary>
    /// Инициализирует базу данных якорных лебёдок.
    /// Проверяет настройку сброса базы, создаёт структуру БД
    /// и при отсутствии данных запускает импорт из Excel.
    /// </summary>
    /// <param name="app">
    /// Конвейер ASP.NET Core, через который получается доступ
    /// к зарегистрированным сервисам приложения.
    /// </param>
    /// <returns>
    /// Исходный экземпляр <see cref="IApplicationBuilder"/>.
    /// </returns>
    public static async Task<IApplicationBuilder> InitializeWinchAnchor(
        this IApplicationBuilder app)
    {
        // Создаём отдельную область DI
        // для работы с DbContext и сервисами инициализации.
        using (var scope =
               app.ApplicationServices.CreateScope())
        {
            var services =
                scope.ServiceProvider;

            try
            {
                Console.WriteLine(
                    "=== START BRAM DATABASE INIT ===");

                var context =
                    services.GetRequiredService<
                        AppDbContextWinchAnchor>();

                var configuration =
                    services.GetRequiredService<
                        IConfiguration>();

                // Определяем, требуется ли принудительно
                // удалить и пересоздать базу SiteBram.
                var resetDatabase =
                    configuration.GetValue<bool>(
                        "ResetWinchAnchorDatabase");

                Console.WriteLine(
                    $"Reset database: {resetDatabase}");

                if (resetDatabase)
                {
                    Console.WriteLine(
                        "Deleting Bram database...");

                    await context.Database
                        .EnsureDeletedAsync();
                }

                // Создаём базу данных и таблицы,
                // если они ещё не существуют.
                await context.Database
                    .EnsureCreatedAsync();

                Console.WriteLine(
                    "Bram database created.");

                // Проверяем наличие основных справочных данных.
                var hasSeries =
                    await context.WinchAnchorSeries
                        .AnyAsync();

                Console.WriteLine(
                    $"Has series: {hasSeries}");

                // Если база была пересоздана
                // или справочник серий пуст,
                // загружаем исходные данные из Excel.
                if (resetDatabase || !hasSeries)
                {
                    Console.WriteLine(
                        "Starting Excel import...");

                    var excelService =
                        services.GetRequiredService<
                            IWinchExcelImportService>();

                    await excelService.ImportAsync();

                    Console.WriteLine(
                        "Excel import completed.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "=== BRAM ERROR ===");

                Console.WriteLine(e);

                // Записываем ошибку инициализации
                // в стандартную систему логирования ASP.NET Core.
                var logger =
                    services.GetRequiredService<
                        ILogger<Program>>();

                logger.LogError(
                    e,
                    "Ошибка при инициализации базы якорных лебедок.");
            }
        }

        return app;
    }
}















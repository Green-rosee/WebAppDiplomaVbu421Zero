using WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;

namespace WebAppEstimate.Areas.SiteAdam.Pipeline;

/// <summary>
///     Выполняет первоначальную инициализацию базы данных SiteAdam.
///     При необходимости пересоздаёт базу и может заполнить её
///     демонстрационными данными.
/// </summary>
public static class DatabaseInitPumpCentrifugal
{
    /// <summary>
    ///     Инициализирует базу данных центробежных насосов.
    ///     Создаёт структуру БД и при включённом режиме
    ///     демонстрационных данных выполняет их генерацию.
    /// </summary>
    /// <param name="app">
    ///     Конвейер ASP.NET Core, через который получается доступ
    ///     к зарегистрированным сервисам приложения.
    /// </param>
    /// <returns>
    ///     Исходный экземпляр <see cref="IApplicationBuilder" />.
    /// </returns>
    public static IApplicationBuilder InitializePumpCentrifugal(
        this IApplicationBuilder app)
    {
        // Создаём отдельную область DI
        // для работы с контекстом базы данных и конфигурацией.
        using var scope =
            app.ApplicationServices.CreateScope();

        var services =
            scope.ServiceProvider;

        try
        {
            var context =
                services.GetRequiredService<
                    AppDbContextCentrifugalPump>();

            var configuration =
                services.GetRequiredService<
                    IConfiguration>();

            // Определяем, требуется ли принудительно
            // удалить и заново создать базу SiteAdam.
            var resetDatabase =
                configuration.GetValue<bool>(
                    "ResetPumpDatabaseAdam");

            // Определяем, нужно ли использовать
            // демонстрационные данные при разработке.
            var useDemoData =
                configuration.GetValue<bool>(
                    "UseDemoPumpData");

            if (resetDatabase) context.Database.EnsureDeleted();

            // Создаём базу данных и таблицы,
            // если они ещё не существуют.
            context.Database.EnsureCreated();

            // Демонстрационные данные загружаются
            // только если это явно разрешено в конфигурации
            // и таблица серий ещё пуста.
            if (useDemoData &&
                !context.PumpSeries.Any())
                DbInitializer.SeedData(context);
        }
        catch (Exception e)
        {
            // Записываем ошибку инициализации
            // через стандартную систему логирования ASP.NET Core.
            var logger =
                services.GetRequiredService<
                    ILogger<Program>>();

            logger.LogError(
                e,
                "Ошибка при инициализации базы данных.");
        }

        return app;
    }
}
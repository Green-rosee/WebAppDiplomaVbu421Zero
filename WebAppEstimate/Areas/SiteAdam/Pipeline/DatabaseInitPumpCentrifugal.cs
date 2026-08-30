using WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;

namespace WebAppEstimate.Areas.SiteAdam.Pipeline;

public static class DatabaseInitPumpCentrifugal
{
    public static IApplicationBuilder InitializePumpCentrifugal(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<AppDbContextCentrifugalPump>();
                //---
                var configuration = services.GetRequiredService<IConfiguration>();
                var resetDatabase = configuration.GetValue<bool>("ResetPumpDatabaseAdam");

                if (resetDatabase) context.Database.EnsureDeleted();

                //---было до упоковки в докер
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                //---
                if (resetDatabase || !context.PumpSeries.Any()) DbInitializer.SeedData(context);


                //===
                // 2. ИСПРАВЛЕНИЕ: Вызываем ваш метод генерации данных Faker, который мы написали ранее
                // (Если метод SeedData находится в другом классе, например DbInitializer, 
                // укажите здесь: DbInitializer.SeedData(context);)
                //SeedData(context);
            }
            catch (Exception e)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(e, "Ошибка при инициализации базы данных.");
            }
        }

        return app;
    }

    private static void SeedData(AppDbContextCentrifugalPump context)
    {
        DbInitializer.SeedData(context);
    }
}
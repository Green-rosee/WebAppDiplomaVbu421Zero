using WebAppEstimate.Data.DbContext;

namespace WebAppEstimate.Pipeline;

public static class DatabaseInitAuthorize
{
    public static IApplicationBuilder InitializeAuthorize(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var configuration = services.GetRequiredService<IConfiguration>();

                var resetDatabase = configuration.GetValue<bool>("ResetAuthorizeDatabase");

                if (resetDatabase) context.Database.EnsureDeleted();

                //---я закоментил так как проект в докере
                //context.Database.EnsureDeleted();
                //
                //---
                context.Database.EnsureCreated();
            }
            catch (Exception e)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(e, "Ошибка при инициализации базы данных.");
            }
        }

        return app;
    }
}
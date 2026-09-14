using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;

namespace WebAppEstimate.Areas.SiteBram.Pipeline;

public static class DatabaseInitWinchAnchor
{
    public static IApplicationBuilder InitializeWinchAnchor(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<AppDbContextWinchAnchor>();
                    var configuration = services.GetRequiredService<IConfiguration>();
                        var resetDatabase = configuration.GetValue<bool>("ResetWinchAnchorDatabase");

                        if (resetDatabase)
                        {
                            context.Database.EnsureDeleted();
                        }
                        
                context.Database.EnsureCreated();
                
                if (resetDatabase || !context.WinchAnchorSeries.Any())
                {
                    WinchAnchorExcelInitializer.SeedData(context);
                }
                
            }
            catch (Exception e)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, "Ошибка при инициализации базы якорных лебедок.");
            }
        }

        return app;
    }
}
using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;
using WebAppEstimate.Areas.SiteBram.Services;

namespace WebAppEstimate.Areas.SiteBram.Pipeline;

public static class DatabaseInitWinchAnchor
{
    public static async Task<IApplicationBuilder> InitializeWinchAnchor(
        this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                Console.WriteLine("=== START BRAM DATABASE INIT ===");
                
                var context = 
                    services.GetRequiredService<AppDbContextWinchAnchor>();
                        var configuration = 
                            services.GetRequiredService<IConfiguration>();
                                var resetDatabase = 
                                    configuration.GetValue<bool>("ResetWinchAnchorDatabase");
                                
                                Console.WriteLine($"Reset database: {resetDatabase}");

                if (resetDatabase)
                {
                    Console.WriteLine("Deleting Bram database...");
                    await context.Database.EnsureDeletedAsync();
                }
                        
                await context.Database.EnsureCreatedAsync();
                Console.WriteLine("Bram database created.");
                
                var hasSeries =
                    await context.WinchAnchorSeries.AnyAsync();

                Console.WriteLine($"Has series: {hasSeries}");
                
                
                if (resetDatabase || !hasSeries)   //!await context.WinchAnchorSeries.AnyAsync()
                {
                    Console.WriteLine("Starting Excel import...");
                    
                    var excelService =
                        services.GetRequiredService<IWinchExcelImportService>();

                    await excelService.ImportAsync();
                    
                    Console.WriteLine("Excel import completed.");
                    //WinchAnchorExcelInitializer.SeedData(context);
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("=== BRAM ERROR ===");
                Console.WriteLine(e);
                
                var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, "Ошибка при инициализации базы якорных лебедок.");
            }
        }

        return app;
    }
}
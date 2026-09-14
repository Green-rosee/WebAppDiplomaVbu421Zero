using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;
using WebAppEstimate.Data.DbContext;

namespace WebAppEstimate.Pipeline;

public static class ServiceDatabaseRegistration
{
    public static IServiceCollection AddApplicationDatabases(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var baseConnectionString =
            configuration.GetConnectionString("dbSiteBaseConnection");

        services.AddDbContext<AppDbContext>(
            options => options.UseSqlite(baseConnectionString));

        var adamConnectionString =
            configuration.GetConnectionString("dbSiteAdamConnection");

        services.AddDbContext<AppDbContextCentrifugalPump>(
            options => options.UseSqlite(adamConnectionString));

        var bramConnectionString =
            configuration.GetConnectionString("dbSiteBramConnection");

        services.AddDbContext<AppDbContextWinchAnchor>(
            options => options.UseSqlite(bramConnectionString));

        return services;
    }
}
using FluentValidation;
using WebAppEstimate.Areas.SiteAdam.Models;
using WebAppEstimate.Areas.SiteAdam.Services;
using WebAppEstimate.Areas.SiteAdam.Services.ExcelPumps;
using WebAppEstimate.Areas.SiteBram.Models;
using WebAppEstimate.Areas.SiteBram.Services;
using WebAppEstimate.Services;

namespace WebAppEstimate.Pipeline.PipelineBuilder;

public static class ServiceApplicationRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddControllersWithViews();

        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddValidatorsFromAssemblyContaining<CentrifugalPumpModelValid>();

        services.AddScoped<ICentrifugalPumpService, CentrifugalPumpService>();
        services.AddScoped<ICentrifugalPumpHistoryService, CentrifugalPumpHistoryService>();
        services.AddScoped<ICentrifugalPumpExcelService, CentrifugalPumpExcelService>();

        services.AddScoped<IWinchExcelImportService, WinchAnchorExcelImportService>();
        services.AddScoped<IWinchAnchorService, WinchAnchorService>();
        services.AddScoped<WinchAnchorExcelState>();
        services.AddScoped<IWinchAnchorHistoryService, WinchAnchorHistoryService>();
        services.AddScoped<IWinchAnchorExcelExportService, WinchAnchorExcelExportService>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
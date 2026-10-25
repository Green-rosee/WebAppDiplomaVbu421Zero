using FluentValidation;
using WebAppEstimate.Areas.SiteAdam.Models;
using WebAppEstimate.Areas.SiteAdam.Services;
using WebAppEstimate.Areas.SiteAdam.Services.ExcelPumps;
using WebAppEstimate.Areas.SiteBram.Models;
using WebAppEstimate.Areas.SiteBram.Services;
using WebAppEstimate.Services;

namespace WebAppEstimate.Pipeline.PipelineBuilder;


/// <summary>
/// Содержит регистрацию основных прикладных сервисов приложения
/// в DI-контейнере ASP.NET Core.
/// </summary>
public static class ServiceApplicationRegistration
{
    /// <summary>
    /// Регистрирует MVC, FluentValidation,
    /// сервисы SiteAdam, SiteBram и сервис создания JWT-токенов.
    /// </summary>
    /// <param name="services">
    /// Коллекция сервисов DI-контейнера ASP.NET Core.
    /// </param>
    /// <returns>
    /// Та же коллекция сервисов для поддержки цепочки вызовов.
    /// </returns>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // Подключаем поддержку MVC-контроллеров и Razor Views.
        services.AddControllersWithViews();

        // Автоматически регистрируем FluentValidation-валидаторы
        // из основной сборки приложения.
        services.AddValidatorsFromAssemblyContaining<Program>();

        // Дополнительно регистрируем валидаторы,
        // связанные с моделями центробежных насосов.
        services.AddValidatorsFromAssemblyContaining<
            CentrifugalPumpModelValid>();


        // ---------------- SiteAdam ----------------

        // Основной сервис расчёта параметров центробежного насоса.
        services.AddScoped<
            ICentrifugalPumpService,
            CentrifugalPumpService>();

        // Сервис работы с историей расчётов центробежных насосов.
        services.AddScoped<
            ICentrifugalPumpHistoryService,
            CentrifugalPumpHistoryService>();

        // Сервис работы с Excel-файлами для SiteAdam.
        services.AddScoped<
            ICentrifugalPumpExcelService,
            CentrifugalPumpExcelService>();
        
        // Сервис для загрузки  Excel-файлов в БД  для SiteAdam.
        services.AddScoped<
            ICentrifugalPumpExcelImportService,
            CentrifugalPumpExcelImportService>();


        // ---------------- SiteBram ----------------

        // Сервис импорта исходных данных якорных лебёдок из Excel.
        services.AddScoped<
            IWinchExcelImportService,
            WinchAnchorExcelImportService>();

        // Основной сервис расчёта и подбора якорной лебёдки.
        services.AddScoped<
            IWinchAnchorService,
            WinchAnchorService>();

        // Хранит состояние Excel-таблицы
        // в пределах текущего DI-scope.
        services.AddScoped<
            WinchAnchorExcelState>();

        // Сервис работы с историей расчётов якорных лебёдок.
        services.AddScoped<
            IWinchAnchorHistoryService,
            WinchAnchorHistoryService>();

        // Сервис формирования и экспорта итогового Excel-документа.
        services.AddScoped<
            IWinchAnchorExcelExportService,
            WinchAnchorExcelExportService>();


        // ---------------- Авторизация ----------------

        // Сервис генерации JWT-токенов
        // для авторизованных пользователей.
        services.AddScoped<
            IJwtTokenService,
            JwtTokenService>();

        return services;
    }
}














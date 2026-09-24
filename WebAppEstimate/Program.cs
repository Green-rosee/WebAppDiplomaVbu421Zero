using FluentValidation;
using Syncfusion.Blazor;
using WebAppEstimate.Areas.SiteAdam.Models;
using WebAppEstimate.Areas.SiteAdam.Pipeline;
using WebAppEstimate.Areas.SiteAdam.Services;
using WebAppEstimate.Areas.SiteAdam.Services.ExcelPumps;
using WebAppEstimate.Areas.SiteBram.Pipeline;
using WebAppEstimate.Areas.SiteBram.Services;
using WebAppEstimate.Components;
using WebAppEstimate.Pipeline;
using WebAppEstimate.Services;

//-------------------------start
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationDatabases(builder.Configuration);
//Регистрируем аутентификацию через JWT-Token
builder.Services.AddServiceAddJwtAuthentication();


// Add services to the container.
//
builder.Services.AddControllersWithViews();

//Добавляем FluentValidation в DI контейнер
//---
builder.Services.AddScoped<ICentrifugalPumpService, CentrifugalPumpService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
//
builder.Services.AddValidatorsFromAssemblyContaining<CentrifugalPumpModelValid>();
builder.Services.AddScoped<ICentrifugalPumpHistoryService,
    CentrifugalPumpHistoryService>();
//
builder.Services.AddScoped<ICentrifugalPumpExcelService,
    CentrifugalPumpExcelService>();
builder.Services.AddScoped<IWinchExcelImportService, WinchAnchorExcelImportService>();
//
builder.Services.AddScoped<IWinchAnchorService, WinchAnchorService>();

//----------Add Jwt Token-------------------
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
//---

//---Добавить Blazor-Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
//----Регестрация библиотеки для Excel
builder.Services.AddSyncfusionBlazor();

var app = builder.Build();

//
//---создание таблицы даных для авторизации входа на саты
app.InitializeAuthorize();
//---создание таблицы даных для заполнения днаыми центробежных насосов
app.InitializePumpCentrifugal();
await app.InitializeWinchAnchor();
//---
app.UsePipeline();
app.MapStaticAssets();

//новая область создана SiteAdam
app.MapAreaControllerRoute(
    "siteAdamArea",
    "SiteAdam",
    "SiteAdam/{controller=Home}/{action=Index}/{id?}"
);

//новая область создана SiteBram
app.MapAreaControllerRoute(
    "siteBramArea",
    "SiteBram",
    "SiteBram/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
//-----------
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
/*app.MapRazorComponents<AppBram>()
    .AddInteractiveServerRenderMode();*/


// 3. УНИВЕРСАЛЬНЫЙ МАРШРУТ ДЛЯ ВСЕХ ОБЛАСТЕЙ (ТЕКУЩИХ И БУДУЩИХ)
/*
app.MapControllerRoute(
    name: "allAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);
*/

//-----
app.Run();
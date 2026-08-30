using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;
using WebAppEstimate.Areas.SiteAdam.Models;
using WebAppEstimate.Areas.SiteAdam.Pipeline;
using WebAppEstimate.Areas.SiteAdam.Services;
using WebAppEstimate.Data.DbContext;
using WebAppEstimate.Pipeline;

//-------------------------start
var builder = WebApplication.CreateBuilder(args);

// 1. ИСПРАВЛЕНИЕ: Регистрируем ДВА РАЗНЫХ контекста для двух разных баз данных
var baseConnectionString = builder.Configuration.GetConnectionString("dbSiteBaseConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(baseConnectionString));

var adamConnectionString = builder.Configuration.GetConnectionString("dbSiteAdamConnection");
// Регистрируем именно контекст для насосов, а не дублируем базовый!
builder.Services.AddDbContext<AppDbContextCentrifugalPump>(options => options.UseSqlite(adamConnectionString));
//
// Регистрируем аутентификацию через Куки
builder.Services.ServiceAuthenticationCookies();


// Add services to the container.
//
builder.Services.AddControllersWithViews();

//Добавляем FluentValidation в DI контейнер
//---
builder.Services.AddScoped<ICentrifugalPumpService, CentrifugalPumpService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddValidatorsFromAssemblyContaining<CentrifugalPumpModelValid>();
builder.Services.AddScoped<ICentrifugalPumpHistoryService, CentrifugalPumpHistoryService>();
//---

var app = builder.Build();
//
//---создание таблицы даных для авторизации входа на саты
app.InitializeAuthorize();
//---создание таблицы даных для заполнения днаыми центробежных насосов
app.InitializePumpCentrifugal();
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

// 3. УНИВЕРСАЛЬНЫЙ МАРШРУТ ДЛЯ ВСЕХ ОБЛАСТЕЙ (ТЕКУЩИХ И БУДУЩИХ)
/*
app.MapControllerRoute(
    name: "allAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);
*/

//-----
app.Run();
using FluentValidation;
using WebAppEstimate.Areas.SiteAdam.Pipeline;
using WebAppEstimate.Areas.SiteBram.Pipeline;
using WebAppEstimate.Areas.SiteCerber.Services;
using WebAppEstimate.Components;
using WebAppEstimate.Pipeline;
using WebAppEstimate.Pipeline.PipelineBuilder;

//-------------------------start
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationDatabases(builder.Configuration);

builder.Services.AddApplicationAuthentication();
builder.Services.AddApplicationServices();
builder.Services.AddApplicationBlazor();

//--------------
builder.Services.AddApplicationServices()
    .AddScoped<IOpenScadService,
        OpenScadService>();
builder.Services.AddHttpClient
<IGigaChatService,
    GigaChatService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped
<IFlangeDrawingService,
    FlangeDrawingService>();
//-----------
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

//новая область создана SiteCerber
app.MapAreaControllerRoute(
    "siteCerberArea",
    "SiteCerber",
    "SiteCerber/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//-----------
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//-----
app.Run();
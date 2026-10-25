using FluentValidation;
using WebAppEstimate.Areas.SiteAdam.Pipeline;
using WebAppEstimate.Areas.SiteBram.Pipeline;
using WebAppEstimate.Areas.SiteCerber.Services;
using WebAppEstimate.Components;
using WebAppEstimate.Pipeline;
using WebAppEstimate.Pipeline.PipelineBuilder;


// Создаём builder приложения ASP.NET Core.
// Через него настраиваются конфигурация, DI-контейнер и сервисы.
var builder = WebApplication.CreateBuilder(args);

System.Text.Encoding.RegisterProvider(
    System.Text.CodePagesEncodingProvider.Instance);

// Регистрируем контексты баз данных.
// В зависимости от конфигурации используются SQLite или PostgreSQL.
builder.Services.AddApplicationDatabases(
    builder.Configuration);

// Регистрируем JWT-аутентификацию и авторизацию.
builder.Services.AddApplicationAuthentication();

// Регистрируем основные сервисы приложения.
builder.Services.AddApplicationServices();

// Регистрируем Razor/Blazor-компоненты.
builder.Services.AddApplicationBlazor();


// ---------------- SiteCerber ----------------

// Регистрируем сервис генерации 3D-моделей через OpenSCAD.
builder.Services.AddScoped<
    IOpenScadService,
    OpenScadService>();

// Регистрируем GigaChatService через HttpClient.
// Сервис используется для обращения к внешнему GigaChat API.
builder.Services.AddHttpClient<
    IGigaChatService,
    GigaChatService>();

// Автоматически регистрируем FluentValidation-валидаторы
// из текущей сборки приложения.
builder.Services
    .AddValidatorsFromAssemblyContaining<Program>();

// Регистрируем сервис генерации SVG-чертежей фланца.
builder.Services.AddScoped<
    IFlangeDrawingService,
    FlangeDrawingService>();


// Создаём приложение после регистрации всех сервисов.
var app = builder.Build();


// ---------------- Инициализация данных ----------------

// Создаём базу авторизации и стандартных пользователей,
// если они ещё не существуют.
app.InitializeAuthorize();

// Инициализируем данные центробежных насосов SiteAdam.
app.InitializePumpCentrifugal();

// Инициализируем данные якорных лебёдок SiteBram.
// Метод асинхронный, так как может выполнять работу с БД и Excel.
await app.InitializeWinchAnchor();


// ---------------- HTTP pipeline ----------------

// Подключаем middleware приложения:
// обработку ошибок, HTTPS, static files,
// authentication, authorization и маршрутизацию.
app.UsePipeline();

// Подключаем статические ресурсы ASP.NET Core.
app.MapStaticAssets();


// ---------------- ASP.NET Core Areas ----------------

// Маршрут функциональной области SiteAdam.
app.MapAreaControllerRoute(
    "siteAdamArea",
    "SiteAdam",
    "SiteAdam/{controller=Home}/{action=Index}/{id?}"
);

// Маршрут функциональной области SiteBram.
app.MapAreaControllerRoute(
    "siteBramArea",
    "SiteBram",
    "SiteBram/{controller=Home}/{action=Index}/{id?}"
);

// Маршрут функциональной области SiteCerber.
app.MapAreaControllerRoute(
    "siteCerberArea",
    "SiteCerber",
    "SiteCerber/{controller=Home}/{action=Index}/{id?}"
);


// Маршрут MVC по умолчанию.
app.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


// ---------------- Blazor ----------------

// Подключаем корневой Blazor-компонент App
// и режим Interactive Server.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


// Запускаем веб-приложение.
app.Run();
















































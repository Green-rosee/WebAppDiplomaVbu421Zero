// <-- ОБЯЗАТЕЛЬНО ДОБАВЬТЕ ДЛЯ IWebHostEnvironment
// <-- ОБЯЗАТЕЛЬНО ДОБАВЬТЕ ДЛЯ GetService

namespace WebAppEstimate.Pipeline;


/// <summary>
/// Настраивает HTTP middleware-конвейер приложения.
/// Определяет порядок обработки ошибок, HTTPS,
/// статических файлов, маршрутизации, аутентификации
/// и авторизации.
/// </summary>
public static class PipeLineMiddleware
{
    /// <summary>
    /// Подключает основные middleware-компоненты приложения.
    /// </summary>
    /// <param name="app">
    /// Конвейер обработки HTTP-запросов ASP.NET Core.
    /// </param>
    /// <returns>
    /// Настроенный экземпляр <see cref="IApplicationBuilder"/>.
    /// </returns>
    public static IApplicationBuilder UsePipeline(
        this IApplicationBuilder app)
    {
        // Получаем информацию о текущем окружении приложения.
        var environment =
            app.ApplicationServices
                .GetRequiredService<IWebHostEnvironment>();

        // В production-окружении используем централизованную
        // обработку исключений и механизм HSTS.
        if (!environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // Обрабатываем HTTP-коды ошибок, например 404 и 403,
        // повторно выполняя запрос к специальной странице ошибки.
        app.UseStatusCodePagesWithReExecute(
            "/Error/{0}");

        // Перенаправляем HTTP-запросы на HTTPS.
        app.UseHttpsRedirection();

        // Разрешаем отдачу статических файлов из wwwroot.
        app.UseStaticFiles();

        // Подключаем маршрутизацию запросов.
        app.UseRouting();

        // Сначала определяем личность пользователя
        // на основе JWT-токена.
        app.UseAuthentication();

        // Затем проверяем права пользователя
        // и доступ к ресурсам согласно его роли.
        app.UseAuthorization();

        // Защита Razor Components / Blazor
        // от CSRF-запросов.
        app.UseAntiforgery();

        return app;
    }
}
















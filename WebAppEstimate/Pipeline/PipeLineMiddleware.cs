// <-- ОБЯЗАТЕЛЬНО ДОБАВЬТЕ ДЛЯ IWebHostEnvironment
// <-- ОБЯЗАТЕЛЬНО ДОБАВЬТЕ ДЛЯ GetService

namespace WebAppEstimate.Pipeline;

public static class PipeLineMiddleware
{
    public static IApplicationBuilder UsePipeline(this IApplicationBuilder app)
    {
        //------ Достаем информацию об окружении (Environment) через ApplicationServices
        var environment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();


        //------Исключения обработка
        if (!environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // Обработка HTTP ошибок:
        // 404, 403 и т.д.
        app.UseStatusCodePagesWithReExecute(
            "/Error/{0}"
        );

        //--------------
        app.UseHttpsRedirection();
        
        app.UseStaticFiles();
        app.UseRouting();
        //---
        // --- ИСПРАВЛЕНИЕ 1: СТРОГИЙ ПОРЯДОК АУТЕНТИФИКАЦИИ ---
        app.UseAuthentication(); // КРИТИЧЕСКИ ВАЖНО! Сначала проверяем КТО пользователь (Проверяем Куки)
        app.UseAuthorization(); // Затем проверяем ЧТО ему разрешено (Проверяем роли, например, Admin)
        
        //--- Нужно для Razor Components / Blazor
        app.UseAntiforgery();
        //app.MapStaticAssets();
        

        return app;
    }
}
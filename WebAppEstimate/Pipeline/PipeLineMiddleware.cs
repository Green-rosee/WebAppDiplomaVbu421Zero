
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting; // <-- ОБЯЗАТЕЛЬНО ДОБАВЬТЕ ДЛЯ IWebHostEnvironment
using Microsoft.Extensions.DependencyInjection; // <-- ОБЯЗАТЕЛЬНО ДОБАВЬТЕ ДЛЯ GetService
using Microsoft.Extensions.Hosting;

namespace WebAppEstimate.Pipeline;

public static class PipeLineMiddleware
{
    public static IApplicationBuilder  UsePipeline(this IApplicationBuilder  app)
    {
        // Достаем информацию об окружении (Environment) через ApplicationServices
        var environment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        //
        
        // Configure the HTTP request pipeline.
        if (!environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        //---
        app.UseHttpsRedirection();
        app.UseRouting();
        //---
        // --- ИСПРАВЛЕНИЕ 1: СТРОГИЙ ПОРЯДОК АУТЕНТИФИКАЦИИ ---
        app.UseAuthentication();    // КРИТИЧЕСКИ ВАЖНО! Сначала проверяем КТО пользователь (Проверяем Куки)
        app.UseAuthorization();     // Затем проверяем ЧТО ему разрешено (Проверяем роли, например, Admin)
        //---
        //app.MapStaticAssets();

        return app;
    }
    
    //---
    
}

/*if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}*/
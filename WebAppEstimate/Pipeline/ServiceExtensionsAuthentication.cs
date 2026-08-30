using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebAppEstimate.Pipeline;

public static class ServiceExtensionsAuthentication
{
    public static IServiceCollection ServiceAuthenticationCookies(this IServiceCollection services,
        string authenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                // Перенаправление на корневую страницу Index, если не авторизован
                options.LoginPath = "/Start/Start";

                // Перенаправление туда же, если авторизован, но не Admin
                options.AccessDeniedPath = "/Start/Start";

                // --- время жизни куки 2 минуты
                options.ExpireTimeSpan = TimeSpan.FromMinutes(2);

                // --- продлевать ли куки при активности пользователя
                options.SlidingExpiration = true;
            });
        //---
        return services;
    }
}
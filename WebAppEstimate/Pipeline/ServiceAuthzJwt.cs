using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WebAppEstimate.Data.Entity;

namespace WebAppEstimate.Pipeline;



/// <summary>
/// Содержит настройку JWT-аутентификации приложения.
/// Определяет правила проверки токена и способ его получения из cookie.
/// </summary>
public static class ServiceAuthzJwt
{
    /// <summary>
    /// Регистрирует JWT Bearer-аутентификацию
    /// и параметры проверки JWT-токенов.
    /// </summary>
    /// <param name="services">
    /// Коллекция сервисов DI-контейнера ASP.NET Core.
    /// </param>
    /// <returns>
    /// Та же коллекция сервисов для поддержки цепочки вызовов.
    /// </returns>
    public static IServiceCollection AddServiceAddJwtAuthentication(
        this IServiceCollection services)
    {
        services
            .AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Настраиваем правила проверки JWT-токена.
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        // Проверяем, что токен создан
                        // ожидаемым издателем.
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,

                        // Проверяем, что токен предназначен
                        // для ожидаемого приложения.
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,

                        // Проверяем срок действия токена.
                        ValidateLifetime = true,

                        // Ключ используется для проверки
                        // цифровой подписи JWT.
                        IssuerSigningKey =
                            AuthOptions.GetSymmetricSecurityKey(),

                        // Явно включаем проверку подписи токена.
                        ValidateIssuerSigningKey = true
                    };

                // По умолчанию JwtBearer ищет токен
                // в заголовке Authorization.
                // В этом приложении JWT хранится в HttpOnly cookie,
                // поэтому извлекаем его из access_token.
                options.Events =
                    new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token =
                                context.Request
                                    .Cookies["access_token"];

                            return Task.CompletedTask;
                        }
                    };
            });

        return services;
    }
}










namespace WebAppEstimate.Pipeline.PipelineBuilder;



/// <summary>
/// Содержит регистрацию механизмов аутентификации приложения
/// в DI-контейнере ASP.NET Core.
/// </summary>
public static class ServiceAuthenticationRegistration
{
    /// <summary>
    /// Подключает JWT-аутентификацию приложения.
    /// </summary>
    /// <param name="services">
    /// Коллекция сервисов DI-контейнера ASP.NET Core.
    /// </param>
    /// <returns>
    /// Та же коллекция сервисов для поддержки цепочки вызовов.
    /// </returns>
    public static IServiceCollection AddApplicationAuthentication(
        this IServiceCollection services)
    {
        // Регистрируем настройки JWT-аутентификации
        // через отдельный extension-метод.
        services.AddServiceAddJwtAuthentication();

        return services;
    }
}

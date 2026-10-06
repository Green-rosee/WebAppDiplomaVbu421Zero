namespace WebAppEstimate.Pipeline.PipelineBuilder;

public static class ServiceAuthenticationRegistration
{
    public static IServiceCollection AddApplicationAuthentication(
        this IServiceCollection services)
    {
        services.AddServiceAddJwtAuthentication();

        return services;
    }
}
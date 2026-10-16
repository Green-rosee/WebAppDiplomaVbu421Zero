using BlazorDatasheet.Extensions;
using Syncfusion.Blazor;

namespace WebAppEstimate.Pipeline.PipelineBuilder;


public static class ServiceBlazorRegistration
{
    public static IServiceCollection AddApplicationBlazor(
        this IServiceCollection services)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        services.AddSyncfusionBlazor();
        services.AddBlazorDatasheet();

        return services;
    }

}















using BlazorServer.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using BlazorServer.Services.Interfaces;

namespace BlazorServer.Analytics;

public static class AddAnalyticsTrackingExtensions
{
    public static IServiceCollection AddAnalyticsTracking(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        // State (per circuit)
        services.AddScoped<IUserActivityStateService, UserActivityStateService>();


        // Queue + Logger + Sink
        services.AddSingleton<AnalyticsChannel>();
        services.AddScoped<IAnalyticsLoggerService, ChannelAnalyticsLogger>();
        services.AddSingleton<IAnalyticsSinkService, LoggerAnalyticsSink>();
        services.AddHostedService<AnalyticsBackgroundService>();


        // Public analytics API for DI and cascading use
        services.AddScoped<IAnalyticsService, AnalyticsService>();


        // Auth
        services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();


        return services;
    }
}
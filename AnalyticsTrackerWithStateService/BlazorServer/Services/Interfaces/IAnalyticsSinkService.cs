using BlazorServer.Models;

namespace BlazorServer.Services.Interfaces;

public interface IAnalyticsSinkService
{
    Task WriteAsync(AnalyticsEvent evt, CancellationToken ct = default);
}
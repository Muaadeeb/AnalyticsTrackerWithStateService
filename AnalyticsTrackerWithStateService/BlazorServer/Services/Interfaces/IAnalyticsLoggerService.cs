using BlazorServer.Models;

namespace BlazorServer.Services.Interfaces;

public interface IAnalyticsLoggerService
{
    ValueTask EnqueueAsync(AnalyticsEvent evt, CancellationToken ct = default);
}

namespace BlazorServer.Services.Interfaces;

public interface IAnalyticsService
{
    Task PageViewAsync(string url, string? title = null, IDictionary<string, object?>? props = null, CancellationToken ct = default);
    Task EventAsync(string name, IDictionary<string, object?>? props = null, CancellationToken ct = default);
}
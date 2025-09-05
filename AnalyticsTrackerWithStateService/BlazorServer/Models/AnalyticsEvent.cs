using BlazorServer.Common.Enums;

namespace BlazorServer.Models;

public sealed record AnalyticsEvent
{
    public required AnalyticsEventType Type { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public string? Name { get; init; }
    public string? SessionId { get; init; }
    public string? CircuitId { get; init; }
    public string? UserId { get; init; }
    public string? UserName { get; init; }
    public string? UserEmail { get; init; }
    public string? IpAddress { get; init; }
    public string? Url { get; init; }
    public string? Title { get; init; }
    public string? Referrer { get; init; }
    public ClientInfo? Client { get; init; }
    public Dictionary<string, object?> Props { get; init; } = new();
}
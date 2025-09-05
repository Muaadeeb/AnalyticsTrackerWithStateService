namespace BlazorServer.Models;

public sealed record ClientInfo(
    string? UserAgent,
    string? Language,
    string? Platform,
    int? ScreenWidth,
    int? ScreenHeight,
    double? DevicePixelRatio,
    string? Timezone,
    string? Referrer,
    string? Url,
    string? Title
);

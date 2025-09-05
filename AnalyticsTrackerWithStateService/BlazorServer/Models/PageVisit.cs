namespace BlazorServer.Models;

public sealed record PageVisit(
    DateTimeOffset When,
    string Url,
    string? Title,
    string? Referrer
);

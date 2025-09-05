using BlazorServer.Models;
using System.Security.Claims;

public interface IUserActivityStateService
{
    string SessionId { get; }
    string? UserId { get; }
    string? UserName { get; }
    string? UserEmail { get; }
    string? IpAddress { get; }
    ClientInfo? Client { get; }
    DateTimeOffset FirstSeen { get; }
    DateTimeOffset LastSeen { get; }
    IReadOnlyList<PageVisit> Visits { get; }


    void InitializeIfNeeded(ClaimsPrincipal? user, string? ipAddress, ClientInfo? client);
    void RecordPageVisit(string url, string? title, string? referrer);
}
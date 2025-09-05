using BlazorServer.Models;
using System.Security.Claims;

namespace BlazorServer.Services;

public sealed class UserActivityStateService : IUserActivityStateService
{
    private readonly object _gate = new();
    private readonly List<PageVisit> _visits = new();


    public string SessionId { get; private set; } = Guid.NewGuid().ToString("n");
    public string? UserId { get; private set; }
    public string? UserName { get; private set; }
    public string? UserEmail { get; private set; }
    public string? IpAddress { get; private set; }
    public ClientInfo? Client { get; private set; }
    public DateTimeOffset FirstSeen { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastSeen { get; private set; } = DateTimeOffset.UtcNow;


    public IReadOnlyList<PageVisit> Visits
    {
        get
        {
            lock (_gate) return _visits.ToList();
        }
    }


    public void InitializeIfNeeded(ClaimsPrincipal? user, string? ipAddress, ClientInfo? client)
    {
        lock (_gate)
        {
            IpAddress ??= ipAddress;
            Client ??= client;
            if (user?.Identity?.IsAuthenticated == true && UserId is null)
            {
                UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.Identity?.Name;
                UserName = user.Identity?.Name ?? UserId;
                UserEmail = user.FindFirst(ClaimTypes.Email)?.Value;
            }
        }
    }


    public void RecordPageVisit(string url, string? title, string? referrer)
    {
        lock (_gate)
        {
            _visits.Add(new PageVisit(DateTimeOffset.UtcNow, url, title, referrer));
            LastSeen = DateTimeOffset.UtcNow;
        }
    }
}

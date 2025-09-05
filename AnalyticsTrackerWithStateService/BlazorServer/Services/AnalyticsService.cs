using BlazorServer.Common.Enums;
using BlazorServer.Models;
using BlazorServer.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace BlazorServer.Services;


public sealed class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsLoggerService _logger;
    private readonly IUserActivityStateService _state;
    private readonly AuthenticationStateProvider _auth;
    private readonly IJSRuntime _js;
    private readonly NavigationManager _nav;
    private readonly IHttpContextAccessor _http;


    private bool _initialized;

    public AnalyticsService(
        IAnalyticsLoggerService logger,
        IUserActivityStateService state,
        AuthenticationStateProvider auth,
        IJSRuntime js,
        NavigationManager nav,
        IHttpContextAccessor http)
    {
        _logger = logger;
        _state = state;
        _auth = auth;
        _js = js;
        _nav = nav;
        _http = http;
    }

    public async Task EnsureInitializedAsync(CancellationToken ct = default)
    {
        if (_initialized) return;


        var authState = await _auth.GetAuthenticationStateAsync();
        var user = authState.User;


        ClientInfo? client = null;
        try
        {
            client = await _js.InvokeAsync<ClientInfo>("analyticsTracker.getClientInfo", ct);
        }
        catch
        {
            // JS not ready; ignore.
        }

        var ip = _http.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        _state.InitializeIfNeeded(user, ip, client);

        await _logger.EnqueueAsync(new AnalyticsEvent
        {
            Type = AnalyticsEventType.SessionStart,
            Timestamp = DateTimeOffset.UtcNow,
            SessionId = _state.SessionId,
            UserId = _state.UserId,
            UserName = _state.UserName,
            UserEmail = _state.UserEmail,
            IpAddress = _state.IpAddress,
            Client = _state.Client,
            Url = _nav.Uri,
            Title = client?.Title,
            Referrer = client?.Referrer
        }, ct);


        _initialized = true;
    }

    public async Task PageViewAsync(string url, string? title = null, IDictionary<string, object?>? props = null, CancellationToken ct = default)
    {
        await EnsureInitializedAsync(ct);
        _state.RecordPageVisit(url, title, _state.Client?.Referrer);


        var evt = new AnalyticsEvent
        {
            Type = AnalyticsEventType.PageView,
            Timestamp = DateTimeOffset.UtcNow,
            SessionId = _state.SessionId,
            UserId = _state.UserId,
            UserName = _state.UserName,
            UserEmail = _state.UserEmail,
            IpAddress = _state.IpAddress,
            Client = _state.Client,
            Url = url,
            Title = title,
            Referrer = _state.Client?.Referrer,
            Props = props is null ? new() : new(props)
        };


        await _logger.EnqueueAsync(evt, ct);
    }

    public async Task EventAsync(string name, IDictionary<string, object?>? props = null, CancellationToken ct = default)
    {
        await EnsureInitializedAsync(ct);


        var evt = new AnalyticsEvent
        {
            Type = AnalyticsEventType.Custom,
            Name = name,
            Timestamp = DateTimeOffset.UtcNow,
            SessionId = _state.SessionId,
            UserId = _state.UserId,
            UserName = _state.UserName,
            UserEmail = _state.UserEmail,
            IpAddress = _state.IpAddress,
            Client = _state.Client,
            Url = _nav.Uri,
            Title = null,
            Referrer = _state.Client?.Referrer,
            Props = props is null ? new() : new(props)
        };


        await _logger.EnqueueAsync(evt, ct);
    }

}


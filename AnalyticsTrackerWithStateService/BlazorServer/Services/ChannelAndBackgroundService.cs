using BlazorServer.Models;
using BlazorServer.Services.Interfaces;
using System.Threading.Channels;

namespace BlazorServer.Services;

public sealed class AnalyticsChannel
{
    public Channel<AnalyticsEvent> Channel { get; } = System.Threading.Channels.Channel.CreateUnbounded<AnalyticsEvent>(
    new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });
}


public sealed class ChannelAnalyticsLogger : IAnalyticsLoggerService
{
    private readonly AnalyticsChannel _channel;


    public ChannelAnalyticsLogger(AnalyticsChannel channel)
    {
        _channel = channel;
    }


    public async ValueTask EnqueueAsync(AnalyticsEvent evt, CancellationToken ct = default)
    {
        await _channel.Channel.Writer.WriteAsync(evt, ct);
    }
}


public sealed class AnalyticsBackgroundService : BackgroundService
{
    private readonly AnalyticsChannel _channel;
    private readonly IAnalyticsSinkService _sink;
    private readonly ILogger<AnalyticsBackgroundService> _logger;


    public AnalyticsBackgroundService(AnalyticsChannel channel, IAnalyticsSinkService sink, ILogger<AnalyticsBackgroundService> logger)
    {
        _channel = channel;
        _sink = sink;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var evt in _channel.Channel.Reader.ReadAllAsync(stoppingToken))
        {
            try { await _sink.WriteAsync(evt, stoppingToken); }
            catch (Exception ex) { _logger.LogError(ex, "Analytics sink failed"); }
        }
    }
}


public sealed class LoggerAnalyticsSink : IAnalyticsSinkService
{
    private readonly ILogger<LoggerAnalyticsSink> _logger;


    public LoggerAnalyticsSink(ILogger<LoggerAnalyticsSink> logger)
    {
        _logger = logger;
    }


    public Task WriteAsync(AnalyticsEvent evt, CancellationToken ct = default)
    {
        // Structured log resembling GA payload
        _logger.LogInformation("ANALYTICS {@Type} {@SessionId} {@UserId} {@UserName} {@Url} {@Title} {@Referrer} {@Props}",
        evt.Type, evt.SessionId, evt.UserId, evt.UserName, evt.Url, evt.Title, evt.Referrer, evt.Props);
        return Task.CompletedTask;
    }
}

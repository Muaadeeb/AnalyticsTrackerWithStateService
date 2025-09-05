// README:
// The service reads authenticated user claims from AuthenticationStateProvider.
// To capture Name/Email, ensure your auth populates ClaimTypes.NameIdentifier and ClaimTypes.Email.
// IP address uses IHttpContextAccessor; for reverse proxies, also enable ForwardedHeaders middleware.
// All analytics are queued and written by LoggerAnalyticsSink; replace IAnalyticsSink to persist to your DB or a vendor.

using BlazorServer.Analytics;
using BlazorServer.Components;
using BlazorServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Analytics services
builder.Services.AddAnalyticsTracking();
builder.Services.AddScoped<IUserActivityStateService, UserActivityStateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

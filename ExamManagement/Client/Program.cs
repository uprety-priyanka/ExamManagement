using ExamManagement.Client;
using ExamManagement.Client.Data;
using ExamManagement.Client.Dependencies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddAuthorizationCore(config =>
{
    var policy = new AuthorizationPolicyBuilder();
});
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 7000;
    config.SnackbarConfiguration.HideTransitionDuration = 400;
    config.SnackbarConfiguration.ShowTransitionDuration = 400;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    config.SnackbarConfiguration.ShowTransitionDuration = 400;
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomEnd;
});

builder.Services.AddMudBlazorDialog();
builder.Services.AddOptions();

builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<EMSAuthenticationStateProvider>());
builder.Services.AddScoped<EMSAuthenticationStateProvider>();

builder.Services.RegisterAllGrpcDependencies(builder.HostEnvironment.BaseAddress);

await builder.Build().RunAsync();

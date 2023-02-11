using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddScoped<IMazeGenerator, KruskalMazeGenerator>()
    .AddScoped<ISettingsService, LocalStorageSettingsService>()
    .AddScoped(typeof(IHistoryService<>), typeof(BrowserHistoryService<>))
    .AddMudServices(config => ConfigureSnackbar(config.SnackbarConfiguration));

static void ConfigureSnackbar(SnackbarConfiguration config)
{
    config.PositionClass = Defaults.Classes.Position.BottomCenter;
    config.VisibleStateDuration = 10000;
    config.HideTransitionDuration = 500;
    config.ShowTransitionDuration = 500;
}

await builder.Build().RunAsync();

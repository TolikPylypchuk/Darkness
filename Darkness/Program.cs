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
    .AddMudServices(config =>
    {
        config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;

        config.SnackbarConfiguration.VisibleStateDuration = 10000;
        config.SnackbarConfiguration.HideTransitionDuration = 500;
        config.SnackbarConfiguration.ShowTransitionDuration = 500;
    });

await builder.Build().RunAsync();

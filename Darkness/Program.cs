using System.Net.Http;
using System.Threading.Tasks;

using Darkness.History;
using Darkness.Maze;
using Darkness.Settings;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using MudBlazor;
using MudBlazor.Services;

namespace Darkness
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services
                .AddScoped(sp => new HttpClient { BaseAddress = new(builder.HostEnvironment.BaseAddress) })
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
        }
    }
}

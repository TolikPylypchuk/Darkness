using System.Net.Http;
using System.Threading.Tasks;

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
                .AddSingleton<IMazeGenerator, MazeGenerator>()
                .AddSingleton<ISettingsService, InMemorySettingsService>()
                .AddMudBlazorDialog()
                .AddMudBlazorResizeListener()
                .AddMudBlazorSnackbar(config =>
                {
                    config.PositionClass = Defaults.Classes.Position.BottomCenter;

                    config.VisibleStateDuration = 10000;
                    config.HideTransitionDuration = 500;
                    config.ShowTransitionDuration = 500;
                });

            await builder.Build().RunAsync();
        }
    }
}

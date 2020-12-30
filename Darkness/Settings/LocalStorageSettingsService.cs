using System;
using System.Threading.Tasks;

using BrowserInterop.Extensions;
using BrowserInterop.Storage;

using Microsoft.JSInterop;

namespace Darkness.Settings
{
    public sealed class LocalStorageSettingsService : SettingsServiceBase
    {
        private const string SettingsKey = "settings";

        private readonly IJSRuntime jsRuntime;

        public LocalStorageSettingsService(IJSRuntime javaScript) =>
            this.jsRuntime = javaScript ?? throw new ArgumentNullException(nameof(javaScript));

        public override async ValueTask<GameSettings> GetSettings()
        {
            var localStorage = await this.GetLocalStorage();

            var settings = await localStorage.GetItem<GameSettings>(SettingsKey);
            if (settings != null)
            {
                return settings;
            }

            var defaultSettings = this.GetDefaultSettings();
            await localStorage.SetItem(SettingsKey, defaultSettings);

            return defaultSettings;
        }

        public override async ValueTask SaveSettings(GameSettings settings)
        {
            var localStorage = await this.GetLocalStorage();
            await localStorage.SetItem(SettingsKey, settings ?? throw new ArgumentNullException(nameof(settings)));
        }

        public override async ValueTask ClearSettigns()
        {
            var localStorage = await this.GetLocalStorage();
            await localStorage.SetItem(SettingsKey, this.GetDefaultSettings());
        }

        private async ValueTask<WindowStorage> GetLocalStorage()
        {
            var window = await this.jsRuntime.Window();
            return window.LocalStorage;
        }
    }
}

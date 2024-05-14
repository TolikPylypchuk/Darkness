using BrowserInterop.Extensions;
using BrowserInterop.Storage;

using Microsoft.JSInterop;

namespace Darkness.Settings;

public sealed class LocalStorageSettingsService(IJSRuntime jsRuntime) : SettingsServiceBase
{
    private const string SettingsKey = "settings";

    private readonly IJSRuntime jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));

    public override async ValueTask<GameSettings> GetSettings()
    {
        var localStorage = await this.GetLocalStorage();

        try
        {
            return await localStorage.GetItem<GameSettings>(SettingsKey);
        } catch
        {
            var defaultSettings = await this.GetDefaultSettings();
            await localStorage.SetItem(SettingsKey, defaultSettings);

            return defaultSettings;
        }
    }

    public override async ValueTask SaveSettings(GameSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        var localStorage = await this.GetLocalStorage();
        await localStorage.SetItem(SettingsKey, settings);
    }

    private async ValueTask<WindowStorage> GetLocalStorage()
    {
        var window = await this.jsRuntime.Window();
        return window.LocalStorage;
    }
}

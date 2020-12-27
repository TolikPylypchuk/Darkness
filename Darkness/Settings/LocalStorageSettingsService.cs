using System;
using System.Threading.Tasks;

using Blazored.LocalStorage;

namespace Darkness.Settings
{
    public sealed class LocalStorageSettingsService : SettingsServiceBase
    {
        private const string SettingsKey = "settings";

        private readonly ILocalStorageService localStorage;

        public LocalStorageSettingsService(ILocalStorageService localStorage) =>
            this.localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));

        public override async ValueTask<GameSettings> GetSettings()
        {
            if (await this.localStorage.ContainKeyAsync(SettingsKey))
            {
                return await this.localStorage.GetItemAsync<GameSettings>(SettingsKey);
            }

            var settings = this.GetDefaultSettings();
            await this.localStorage.SetItemAsync(SettingsKey, settings);

            return settings;
        }

        public override ValueTask SaveSettings(GameSettings settings) =>
            this.localStorage.SetItemAsync(SettingsKey, settings ?? throw new ArgumentNullException(nameof(settings)));

        public override ValueTask ClearSettigns() =>
            this.localStorage.SetItemAsync(SettingsKey, this.GetDefaultSettings());
    }
}

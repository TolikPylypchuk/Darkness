namespace Darkness.Settings
{
    public abstract class SettingsService : ISettingsService
    {
        public abstract GameSettings GetSettings();
        public abstract void SaveSettings(GameSettings settings);
        public abstract void ClearSettigns(GameSettings settings);

        protected GameSettings GetDefaultSettings() =>
            new(100, 50);
    }
}

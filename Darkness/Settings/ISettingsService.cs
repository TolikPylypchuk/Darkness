namespace Darkness.Settings
{
    public interface ISettingsService
    {
        public GameSettings GetSettings();
        public void SaveSettings(GameSettings settings);
        public void ClearSettigns(GameSettings settings);
    }
}

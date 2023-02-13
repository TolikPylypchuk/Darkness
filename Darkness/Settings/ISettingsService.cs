namespace Darkness.Settings;

public interface ISettingsService
{
    public ValueTask<GameSettings> GetSettings();

    public ValueTask<GameSettings> GetDefaultSettings();

    public ValueTask SaveSettings(GameSettings settings);
}

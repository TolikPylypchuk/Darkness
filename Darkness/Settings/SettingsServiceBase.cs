namespace Darkness.Settings;

public abstract class SettingsServiceBase : ISettingsService
{
    public abstract ValueTask<GameSettings> GetSettings();
    public abstract ValueTask SaveSettings(GameSettings settings);

    public virtual ValueTask<GameSettings> GetDefaultSettings() =>
        ValueTask.FromResult(new GameSettings(25, 25, true));
}

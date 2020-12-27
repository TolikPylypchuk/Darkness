using System.Threading.Tasks;

namespace Darkness.Settings
{
    public abstract class SettingsServiceBase : ISettingsService
    {
        public abstract ValueTask<GameSettings> GetSettings();
        public abstract ValueTask SaveSettings(GameSettings settings);
        public abstract ValueTask ClearSettigns();

        protected GameSettings GetDefaultSettings() =>
            new(100, 50);
    }
}

using System.Threading.Tasks;

namespace Darkness.Settings
{
    public interface ISettingsService
    {
        public ValueTask<GameSettings> GetSettings();
        public ValueTask SaveSettings(GameSettings settings);
        public ValueTask ClearSettigns();
    }
}

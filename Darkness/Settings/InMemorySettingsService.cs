using System;

namespace Darkness.Settings
{
    public sealed class InMemorySettingsService : SettingsService
    {
        private GameSettings settings;

        public override GameSettings GetSettings() => throw new NotImplementedException();
        public override void SaveSettings(GameSettings settings) => throw new NotImplementedException();

        public override void ClearSettigns(GameSettings settings) => throw new NotImplementedException();
    }
}

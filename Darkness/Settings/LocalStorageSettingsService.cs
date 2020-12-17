using System;

namespace Darkness.Settings
{
    public sealed class LocalStorageSettingsService : ISettingsService
    {
        public GameSettings GetSettings() =>
            throw new NotImplementedException();

        public void SaveSettings(GameSettings settings) =>
            throw new NotImplementedException();

        public void ClearSettigns(GameSettings settings) =>
            throw new NotImplementedException();
    }
}

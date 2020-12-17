namespace Darkness.Settings
{
    public sealed record GameSettings(int MazeWidth, int MazeHeight);

    public static class GameSettingsExtensions
    {
        public static GameSettingsBuilder Builder(this GameSettings settings) =>
            new(settings);
    }
}

namespace Darkness.Settings
{
    public sealed class GameSettingsBuilder
    {
        public int MazeWidth { get; set; }
        public int MazeHeight { get; set; }

        public GameSettingsBuilder(GameSettings settings)
        {
            this.MazeWidth = settings.MazeWidth;
            this.MazeHeight = settings.MazeHeight;
        }

        public GameSettings Build() =>
            new(this.MazeWidth, this.MazeHeight);
    }
}

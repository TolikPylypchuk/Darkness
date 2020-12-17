using Darkness.Settings;

namespace Darkness.Maze
{
    public sealed class MazeGenerator : IMazeGenerator
    {
        public GameMaze CreateMaze(GameSettings settings) =>
            new();
    }
}

using Darkness.Settings;

namespace Darkness.Maze
{
    public interface IMazeGenerator
    {
        public GameMaze CreateMaze(GameSettings settings);
    }
}

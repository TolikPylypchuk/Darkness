using Darkness.Maze;
using Darkness.Settings;

namespace Darkness
{
    public sealed record GameState(GameMaze Maze, GameSettings Settings);
}

using System.Threading;
using System.Threading.Tasks;

using Darkness.Settings;

namespace Darkness.Maze
{
    public interface IMazeGenerator
    {
        public Task<GameMaze> CreateMaze(GameSettings settings, CancellationToken cancellationToken);
    }
}

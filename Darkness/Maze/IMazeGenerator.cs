namespace Darkness.Maze;

public interface IMazeGenerator
{
    public Task<GameMaze> CreateMaze(GameSettings settings, CancellationToken cancellationToken);
}

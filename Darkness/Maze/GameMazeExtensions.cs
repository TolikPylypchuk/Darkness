namespace Darkness.Maze;

public static class GameMazeExtensions
{
    public static bool IsOpen(this CellSide side) =>
        side == CellSide.Passage;

    public static T Get<T>(this T[,] array, Location location) =>
        array[location.Row, location.Column];

    public static void Set<T>(this T[,] array, Location location, T value) =>
        array[location.Row, location.Column] = value;
}

using System.Text;

namespace Darkness.Maze
{
    public sealed record Location(int Row, int Column);

    public enum CellSide { Passage, Wall }

    public sealed record Cell(Location Location, CellSide Left, CellSide Top, CellSide Right, CellSide Bottom)
    {
        public static Cell Closed(Location location) =>
            new(location, CellSide.Wall, CellSide.Wall, CellSide.Wall, CellSide.Wall);
    }

    public enum PlayerDirection { Left, Up, Right, Down }
}

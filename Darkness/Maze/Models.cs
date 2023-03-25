namespace Darkness.Maze;

public sealed record Location(int Row, int Column);

public enum CellSide { Passage, Wall }

public sealed record Cell(Location Location, CellSide Left, CellSide Top, CellSide Right, CellSide Bottom)
{
    public static Cell Closed(Location location) =>
        new(location, CellSide.Wall, CellSide.Wall, CellSide.Wall, CellSide.Wall);

    public int NumberOfWalls() =>
        this.SideToInt(this.Left) + this.SideToInt(this.Top) + this.SideToInt(this.Right) + this.SideToInt(this.Bottom);

    private int SideToInt(CellSide cellSide) =>
        cellSide == CellSide.Wall ? 1 : 0;
}

public enum PlayerDirection { Left, Up, Right, Down }

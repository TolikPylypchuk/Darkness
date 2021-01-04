using System.Text;

namespace Darkness.Maze
{
    public enum CellSide { Passage, Wall }

    public sealed record Cell(int Row, int Column, CellSide Left, CellSide Top, CellSide Right, CellSide Bottom)
    {
        public static Cell Closed(int row, int column) =>
            new(row, column, CellSide.Wall, CellSide.Wall, CellSide.Wall, CellSide.Wall);
    }

    public sealed record GameMaze(Cell[,] Cells);

    public static class GameMazeExtensions
    {
        public static string ToSimpleString(this GameMaze maze)
        {
            var builder = new StringBuilder();
            var cells = maze.Cells;

            var numRows = cells.GetLength(0);
            var numCols = cells.GetLength(1);

            builder.Append(new string('-', numCols * 3 + 1)).AppendLine();

            for (int row = 0; row < numRows; row++)
            {
                builder.Append('|');

                for (int col = 0; col < numCols; col++)
                {
                    builder.Append("  ").Append(cells[row, col].Right.IsOpen() ? ' ' : '|');
                }

                builder.AppendLine().Append('-');

                for (int col = 0; col < numCols; col++)
                {
                    builder.Append(cells[row, col].Bottom.IsOpen() ? "  " : "--").Append('-');
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        public static bool IsOpen(this CellSide side) =>
            side == CellSide.Passage;
    }
}

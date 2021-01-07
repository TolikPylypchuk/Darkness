using System.Text;

namespace Darkness.Maze
{
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

        public static T Get<T>(this T[,] array, Location location) =>
            array[location.Row, location.Column];

        public static void Set<T>(this T[,] array, Location location, T value) =>
            array[location.Row, location.Column] = value;
    }
}

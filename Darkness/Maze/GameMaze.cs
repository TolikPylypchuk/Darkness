using System;

namespace Darkness.Maze
{
    public sealed record GameMaze(Cell[,] Cells, Cell Start, Cell Finish)
    {
        public Cell? GetCellToLeft(Cell cell) =>
            cell.Left.IsOpen() && cell.Location.Column != 0
                ? this.Cells[cell.Location.Row, cell.Location.Column - 1]
                : null;

        public Cell? GetCellToRight(Cell cell) =>
            cell.Right.IsOpen() && cell.Location.Column != this.Cells.GetLength(1) - 1
                ? this.Cells[cell.Location.Row, cell.Location.Column + 1]
                : null;

        public Cell? GetUpperCell(Cell cell) =>
            cell.Top.IsOpen() && cell.Location.Row != 0
                ? this.Cells[cell.Location.Row - 1, cell.Location.Column]
                : null;

        public Cell? GetLowerCell(Cell cell) =>
            cell.Bottom.IsOpen() && cell.Location.Row != this.Cells.GetLength(0) - 1
                ? this.Cells[cell.Location.Row + 1, cell.Location.Column]
                : null;

        public Cell? GetNextCell(Cell cell, PlayerDirection direction) =>
            direction switch
            {
                PlayerDirection.Left => this.GetCellToLeft(cell),
                PlayerDirection.Right => this.GetCellToRight(cell),
                PlayerDirection.Up => this.GetUpperCell(cell),
                PlayerDirection.Down => this.GetLowerCell(cell),
                _ => throw new ArgumentOutOfRangeException(nameof(direction))
            };

        public (Cell?, Cell?) GetOrthogonalCells(Cell cell, PlayerDirection direction) =>
            direction switch
            {
                PlayerDirection.Left or PlayerDirection.Right =>
                    (this.GetUpperCell(cell), this.GetLowerCell(cell)),
                PlayerDirection.Up or PlayerDirection.Down =>
                    (this.GetCellToLeft(cell), this.GetCellToRight(cell)),
                _ => throw new ArgumentOutOfRangeException(nameof(direction))
            };
    }
}

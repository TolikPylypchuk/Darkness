using System;
using System.Collections.Generic;

using Darkness.Settings;

namespace Darkness.Maze
{
    public sealed class KruskalMazeGenerator : IMazeGenerator
    {
        private sealed record Edge(int FirstRow, int FirstColumn, int SecondRow, int SecondColumn);

        public GameMaze CreateMaze(GameSettings settings)
        {
            var (cells, sets) = this.InitializeCells(settings);
            var edges = this.CreateEdges(cells.GetLength(0), cells.GetLength(1));

            while (edges.TryDequeue(out var edge))
            {
                var firstSet = sets[edge.FirstRow, edge.FirstColumn];
                var secondSet = sets[edge.SecondRow, edge.SecondColumn];

                if (!ReferenceEquals(firstSet, secondSet))
                {
                    var firstCell = cells[edge.FirstRow, edge.FirstColumn];
                    var secondCell = cells[edge.SecondRow, edge.SecondColumn];

                    var (newFirstCell, newSecondCell) = this.ConnectCells(firstCell, secondCell);

                    cells[edge.FirstRow, edge.FirstColumn] = newFirstCell;
                    cells[edge.SecondRow, edge.SecondColumn] = newSecondCell;

                    firstSet.Remove(firstCell);
                    secondSet.Remove(secondCell);

                    firstSet.Add(newFirstCell);
                    firstSet.Add(newSecondCell);

                    foreach (var cell in secondSet)
                    {
                        firstSet.Add(cell);
                    }

                    int numRows = sets.GetLength(0);
                    int numCols = sets.GetLength(1);

                    for (int row = 0; row < numRows; row++)
                    {
                        for (int col = 0; col < numCols; col++)
                        {
                            if (ReferenceEquals(sets[row, col], secondSet))
                            {
                                sets[row, col] = firstSet;
                            }
                        }
                    }
                }
            }

            return new(cells);
        }

        private (Cell[,], HashSet<Cell>[,]) InitializeCells(GameSettings settings)
        {
            var cells = new Cell[settings.MazeHeight, settings.MazeWidth];
            var sets = new HashSet<Cell>[settings.MazeHeight, settings.MazeWidth];

            for (int row = 0; row < settings.MazeHeight; row++)
            {
                for (int col = 0; col < settings.MazeWidth; col++)
                {
                    var cell = Cell.Closed(row, col);
                    cells[row, col] = cell;
                    sets[row, col] = new HashSet<Cell> { cell };
                }
            }

            return (cells, sets);
        }

        private Queue<Edge> CreateEdges(int numRows, int numCols)
        {
            var edges = new List<Edge>();

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    if (row > 0)
                    {
                        edges.Add(new(row, col, row - 1, col));
                    }

                    if (col > 0)
                    {
                        edges.Add(new(row, col, row, col - 1));
                    }
                }
            }

            edges.Shuffle(new Random());

            return new(edges);
        }

        private (Cell, Cell) ConnectCells(Cell firstCell, Cell secondCell)
        {
            if (firstCell.Row == secondCell.Row && firstCell.Column + 1 == secondCell.Column)
            {
                return (firstCell with { Right = CellSide.Passage }, secondCell with { Left = CellSide.Passage });
            } else if (firstCell.Row == secondCell.Row && firstCell.Column == secondCell.Column + 1)
            {
                return (firstCell with { Left = CellSide.Passage }, secondCell with { Right = CellSide.Passage });
            } else if (firstCell.Row == secondCell.Row + 1 && firstCell.Column == secondCell.Column)
            {
                return (firstCell with { Top = CellSide.Passage }, secondCell with { Bottom = CellSide.Passage });
            } else if (firstCell.Row + 1 == secondCell.Row && firstCell.Column == secondCell.Column)
            {
                return (firstCell with { Bottom = CellSide.Passage }, secondCell with { Top = CellSide.Passage });
            } else
            {
                throw new ArgumentException(
                    $"Cells ({firstCell.Column}, {firstCell.Row}) and " +
                    $"({secondCell.Column}, {secondCell.Row}) are not adjacent");
            }
        }
    }
}

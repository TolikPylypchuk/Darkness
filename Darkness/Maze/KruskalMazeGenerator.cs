namespace Darkness.Maze;

public sealed class KruskalMazeGenerator : IMazeGenerator
{
    private sealed record Edge(Location FirstLocation, Location SecondLocation);

    public async Task<GameMaze> CreateMaze(GameSettings settings, CancellationToken cancellationToken)
    {
        var random = new Random();

        var (cells, sets) = InitializeCells(settings);

        int numRows = cells.GetLength(0);
        int numCols = cells.GetLength(1);

        var edges = CreateEdges(numRows, numCols, random);

        int i = 0;
        while (edges.TryDequeue(out var edge))
        {
            if (i++ % 10 == 0)
            {
                await Task.Delay(1, cancellationToken);
            }

            var firstSet = sets.Get(edge.FirstLocation);
            var secondSet = sets.Get(edge.SecondLocation);

            if (ReferenceEquals(firstSet, secondSet))
            {
                continue;
            }

            var firstCell = cells.Get(edge.FirstLocation);
            var secondCell = cells.Get(edge.SecondLocation);

            var (newFirstCell, newSecondCell) = ConnectCells(firstCell, secondCell);

            cells.Set(edge.FirstLocation, newFirstCell);
            cells.Set(edge.SecondLocation, newSecondCell);

            firstSet.Remove(firstCell);
            secondSet.Remove(secondCell);

            firstSet.Add(newFirstCell);
            firstSet.Add(newSecondCell);

            foreach (var cell in secondSet)
            {
                firstSet.Add(cell);
            }

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

        var start = cells[random.Next(numRows), 0];
        var end = cells[random.Next(numRows), numCols - 1];

        return new GameMaze(cells, start, end);
    }

    private static (Cell[,], HashSet<Cell>[,]) InitializeCells(GameSettings settings)
    {
        var cells = new Cell[settings.MazeHeight, settings.MazeWidth];
        var sets = new HashSet<Cell>[settings.MazeHeight, settings.MazeWidth];

        for (int row = 0; row < settings.MazeHeight; row++)
        {
            for (int col = 0; col < settings.MazeWidth; col++)
            {
                var cell = Cell.Closed(new Location(row, col));
                cells[row, col] = cell;
                sets[row, col] = new HashSet<Cell> { cell };
            }
        }

        return (cells, sets);
    }

    private static Queue<Edge> CreateEdges(int numRows, int numCols, Random random)
    {
        var edges = new List<Edge>();

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                if (row > 0)
                {
                    edges.Add(new Edge(new Location(row, col), new Location(row - 1, col)));
                }

                if (col > 0)
                {
                    edges.Add(new Edge(new Location(row, col), new Location(row, col - 1)));
                }
            }
        }

        edges.Shuffle(random);

        return new Queue<Edge>(edges);
    }

    private static (Cell, Cell) ConnectCells(Cell firstCell, Cell secondCell)
    {
        var firstLocation = firstCell.Location;
        var secondLocation = secondCell.Location;

        if (firstLocation.Row == secondLocation.Row && firstLocation.Column + 1 == secondLocation.Column)
        {
            return (firstCell with { Right = CellSide.Passage }, secondCell with { Left = CellSide.Passage });
        }

        if (firstLocation.Row == secondLocation.Row && firstLocation.Column == secondLocation.Column + 1)
        {
            return (firstCell with { Left = CellSide.Passage }, secondCell with { Right = CellSide.Passage });
        }

        if (firstLocation.Row == secondLocation.Row + 1 && firstLocation.Column == secondLocation.Column)
        {
            return (firstCell with { Top = CellSide.Passage }, secondCell with { Bottom = CellSide.Passage });
        }

        if (firstLocation.Row + 1 == secondLocation.Row && firstLocation.Column == secondLocation.Column)
        {
            return (firstCell with { Bottom = CellSide.Passage }, secondCell with { Top = CellSide.Passage });
        }

        throw new ArgumentException(
            $"Cells ({firstLocation.Column}, {firstLocation.Row}) and " +
            $"({secondLocation.Column}, {secondLocation.Row}) are not adjacent");
    }
}

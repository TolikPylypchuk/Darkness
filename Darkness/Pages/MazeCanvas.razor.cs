using Microsoft.AspNetCore.Components;

namespace Darkness.Pages;

public partial class MazeCanvas : ComponentBase
{
    [Parameter]
    public required GameSettings Settings { get; set; }

    [Parameter]
    public required GameMaze Maze { get; set; }

    [Parameter]
    public required EventCallback Finished { get; set; }

    private Cell CurrentCell { get; set; } = null!;
    private PlayerDirection CurrentDirection { get; set; } = PlayerDirection.Right;

    private bool IsFinished { get; set; } = false;

    private HashSet<Cell> VisibleCells { get; } = [];
    private HashSet<Cell> PartiallyVisibleCells { get; } = [];
    private HashSet<(Cell, double)> AnimatingCells { get; } = [];

    private string MazeStyle { get; set; } = String.Empty;

    public async Task Move(PlayerDirection direction)
    {
        if (this.IsFinished)
        {
            return;
        }

        if (direction == this.CurrentDirection)
        {
            await this.Move();
        } else
        {
            this.Turn(direction);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            return;
        }

        this.MazeStyle = this.CreateMazeStyle();
        this.CurrentCell = this.Maze.Start;
        this.RecalculateVisibilities();
    }

    private string CreateMazeStyle() =>
        $"--maze-width: {this.Settings.MazeWidth}; --maze-height: {this.Settings.MazeHeight}";

    private void RecalculateVisibilities()
    {
        this.VisibleCells.Clear();
        this.PartiallyVisibleCells.Clear();

        var currentCell = this.CurrentCell;

        do
        {
            this.VisibleCells.Add(currentCell);

            var (cell1, cell2) = this.Maze.GetOrthogonalCells(currentCell, this.CurrentDirection);

            if (!this.Settings.AlwaysShowFinish || !Equals(cell1, this.Maze.Finish))
            {
                this.PartiallyVisibleCells.AddIfNotNull(cell1);
            }

            if (!this.Settings.AlwaysShowFinish || !Equals(cell2, this.Maze.Finish))
            {
                this.PartiallyVisibleCells.AddIfNotNull(cell2);
            }

            currentCell = this.Maze.GetNextCell(currentCell, this.CurrentDirection);
        } while (currentCell != null);
    }

    private async Task Move()
    {
        var nextCell = this.Maze.GetNextCell(this.CurrentCell, this.CurrentDirection);

        if (nextCell != null)
        {
            var (cell1, cell2) = this.Maze.GetOrthogonalCells(this.CurrentCell, this.CurrentDirection);

            this.VisibleCells.Remove(this.CurrentCell);
            this.PartiallyVisibleCells.RemoveIfNotNull(cell1);
            this.PartiallyVisibleCells.RemoveIfNotNull(cell2);

            this.CurrentCell = nextCell;

            if (Equals(this.CurrentCell, this.Maze.Finish))
            {
                await this.Finish();
            }
        }
    }

    private void Turn(PlayerDirection direction)
    {
        this.CurrentDirection = direction;
        this.RecalculateVisibilities();
    }

    private async Task Finish()
    {
        this.IsFinished = true;

        await this.Finished.InvokeAsync();

        this.VisibleCells.Clear();
        this.PartiallyVisibleCells.Clear();

        this.VisibleCells.Add(this.Maze.Finish);

        this.ShowCells(this.BreadthFirstFromStart());
    }

    private List<Cell> BreadthFirstFromStart()
    {
        var random = new Random();

        var cells = new Queue<Cell>();
        cells.Enqueue(this.Maze.Start);

        var result = new List<Cell>(this.Maze.Cells.Length);

        while (cells.Count > 0)
        {
            var currentCell = cells.Dequeue();

            result.Add(currentCell);

            var nextCells = new List<Cell>();
            nextCells.AddIfNotNull(this.Maze.GetUpperCell(currentCell));
            nextCells.AddIfNotNull(this.Maze.GetLowerCell(currentCell));
            nextCells.AddIfNotNull(this.Maze.GetCellToLeft(currentCell));
            nextCells.AddIfNotNull(this.Maze.GetCellToRight(currentCell));
            nextCells.Shuffle(random);

            foreach (var cell in nextCells.Where(c => !result.Contains(c)))
            {
                cells.Enqueue(cell);
            }
        }

        return result;
    }

    private void ShowCells(ICollection<Cell> cells)
    {
        double delay = 0.0;

        foreach (var cell in cells.Where(cell => cell != this.Maze.Finish))
        {
            this.AnimatingCells.Add((cell, delay));
            delay += 0.01;
        }

        this.StateHasChanged();
    }
}

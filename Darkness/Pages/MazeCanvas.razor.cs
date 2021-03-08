using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Darkness.Maze;
using Darkness.Settings;

using Microsoft.AspNetCore.Components;

namespace Darkness.Pages
{
    public partial class MazeCanvas : ComponentBase
    {
        [Parameter]
        public GameSettings Settings { get; set; } = null!;

        [Parameter]
        public GameMaze Maze { get; set; } = null!;

        [Parameter]
        public EventCallback Finished { get; set; }

        private Cell CurrentCell { get; set; } = null!;
        private PlayerDirection CurrentDirection { get; set; } = PlayerDirection.Right;

        private bool IsFinished { get; set; } = false;

        private HashSet<Cell> VisibleCells { get; } = new();
        private HashSet<Cell> PartiallyVisibleCells { get; } = new();

        private double MazeWidth { get; set; }
        private double MazeHeight { get; set; }

        private double CellWidth { get; set; }
        private double CellHeight { get; set; }

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

            double mazeWidth = this.Settings.MazeWidth;
            double mazeHeight = this.Settings.MazeHeight;

            this.MazeWidth = mazeWidth / mazeHeight * 100;
            this.MazeHeight = mazeHeight / mazeWidth * 100;

            this.CellWidth = 1.0 / mazeWidth * 100;
            this.CellHeight = 1.0 / mazeHeight * 100;

            this.CurrentCell = this.Maze.Start;
            this.RecalculateVisiblities();
        }

        private void RecalculateVisiblities()
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
            this.RecalculateVisiblities();
        }

        private async Task Finish()
        {
            this.IsFinished = true;

            await this.Finished.InvokeAsync();

            this.VisibleCells.Clear();
            this.PartiallyVisibleCells.Clear();

            this.VisibleCells.Add(this.Maze.Finish);

            await this.ShowCells(this.BreadthFirstFromStart());
        }

        private IEnumerable<Cell> BreadthFirstFromStart()
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

        private async Task ShowCells(IEnumerable<Cell> cells)
        {
            const int showIncrement = 15;

            int currentIteration = 0;
            int showIteration = showIncrement;

            foreach (var cell in cells)
            {
                this.VisibleCells.Add(cell);

                if (currentIteration++ % showIteration == 0)
                {
                    this.StateHasChanged();
                    await Task.Delay(16);
                    showIteration += showIncrement;
                }
            }

            this.StateHasChanged();
        }
    }
}

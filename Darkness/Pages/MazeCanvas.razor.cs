using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Darkness.Maze;
using Darkness.Settings;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor;

namespace Darkness.Pages
{
    public partial class MazeCanvas : ComponentBase
    {
        public const string ArrowLeft = "ArrowLeft";
        public const string ArrowRight = "ArrowRight";
        public const string ArrowUp = "ArrowUp";
        public const string ArrowDown = "ArrowDown";

        [Parameter]
        public EventCallback OnBackToMainMenuCallback { get; set; }

        [Inject]
        private ISettingsService SettingsService { get; set; } = null!;

        [Inject]
        private IMazeGenerator MazeGenerator { get; set; } = null!;

        [Inject]
        private ISnackbar Snackbar { get; set; } = null!;

        private ElementReference MazeWrapper { get; set; }

        private bool IsLoaded { get; set; } = false;
        private CancellationTokenSource MazeGenerationTokenSource { get; set; } = new();

        private GameMaze Maze { get; set; } = null!;
        private Cell CurrentCell { get; set; } = null!;
        private PlayerDirection CurrentDirection { get; set; } = PlayerDirection.Right;

        private bool IsFinished { get; set; } = false;

        private HashSet<Cell> VisibleCells { get; } = new();
        private HashSet<Cell> PartiallyVisibleCells { get; } = new();

        private double MazeWidth { get; set; }
        private double MazeHeight { get; set; }

        private double CellWidth { get; set; }
        private double CellHeight { get; set; }

        public void CancelIfGeneratingMaze()
        {
            this.MazeGenerationTokenSource.Cancel();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var settings = await this.SettingsService.GetSettings();

            double mazeWidth = settings.MazeWidth;
            double mazeHeight = settings.MazeHeight;

            this.MazeWidth = mazeWidth / mazeHeight * 100;
            this.MazeHeight = mazeHeight / mazeWidth * 100;

            this.CellWidth = 1.0 / mazeWidth * 100;
            this.CellHeight = 1.0 / mazeHeight * 100;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var settings = await this.SettingsService.GetSettings();

                this.StateHasChanged();

                try
                {
                    this.Maze = await this.MazeGenerator.CreateMaze(settings, this.MazeGenerationTokenSource.Token);
                    this.CurrentCell = this.Maze.Start;

                    this.RecalculateVisiblities();

                    this.IsLoaded = true;
                    this.StateHasChanged();

                    await this.MazeWrapper.FocusAsync();
                } catch (TaskCanceledException)
                { }
            }
        }

        private async Task BackToMainMenu()
        {
            this.CancelIfGeneratingMaze();
            await this.OnBackToMainMenuCallback.InvokeAsync();
        }

        private void RecalculateVisiblities()
        {
            this.VisibleCells.Clear();
            this.PartiallyVisibleCells.Clear();

            this.VisibleCells.Add(this.Maze.Finish);

            var currentCell = this.CurrentCell;

            do
            {
                this.VisibleCells.Add(currentCell);

                var (cell1, cell2) = this.Maze.GetOrthogonalCells(currentCell, this.CurrentDirection);

                if (!Equals(cell1, this.Maze.Finish))
                {
                    this.PartiallyVisibleCells.AddIfNotNull(cell1);
                }

                if (!Equals(cell2, this.Maze.Finish))
                {
                    this.PartiallyVisibleCells.AddIfNotNull(cell2);
                }

                currentCell = this.Maze.GetNextCell(currentCell, this.CurrentDirection);
            } while (currentCell != null);
        }

        private Task OnKeyDown(KeyboardEventArgs e) =>
            this.Move(
                e.Key switch
                {
                    ArrowLeft => PlayerDirection.Left,
                    ArrowRight => PlayerDirection.Right,
                    ArrowUp => PlayerDirection.Up,
                    ArrowDown => PlayerDirection.Down,
                    _ => null
                });

        private async Task Move(PlayerDirection? direction)
        {
            if (this.IsFinished)
            {
                return;
            }

            if (direction == this.CurrentDirection)
            {
                await this.Move();
            } else if (direction is PlayerDirection directionToTurn)
            {
                this.Turn(directionToTurn);
            }
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

            this.Snackbar.Add("You won!", Severity.Success);

            this.VisibleCells.Clear();
            this.PartiallyVisibleCells.Clear();

            var random = new Random();

            var cellsToShow = new Queue<Cell>();
            cellsToShow.Enqueue(this.Maze.Start);

            int i = 0;
            while (cellsToShow.Count > 0)
            {
                if (i++ % 10 == 0)
                {
                    this.StateHasChanged();
                    await Task.Delay(10);
                }

                var currentCell = cellsToShow.Dequeue();

                this.VisibleCells.Add(currentCell);

                var nextCells = new List<Cell>();
                nextCells.AddIfNotNull(this.Maze.GetUpperCell(currentCell));
                nextCells.AddIfNotNull(this.Maze.GetLowerCell(currentCell));
                nextCells.AddIfNotNull(this.Maze.GetCellToLeft(currentCell));
                nextCells.AddIfNotNull(this.Maze.GetCellToRight(currentCell));
                nextCells.Shuffle(random);

                foreach (var cell in nextCells.Where(c => !this.VisibleCells.Contains(c)))
                {
                    cellsToShow.Enqueue(cell);
                }
            }
        }
    }
}

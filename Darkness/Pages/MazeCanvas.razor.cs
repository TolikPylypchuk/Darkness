using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Darkness.Maze;
using Darkness.Settings;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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

        private ElementReference MazeWrapper { get; set; }

        private bool IsLoaded { get; set; } = false;
        private CancellationTokenSource MazeGenerationTokenSource { get; set; } = new();

        private GameMaze Maze { get; set; } = null!;
        private Cell CurrentCell { get; set; } = null!;
        private PlayerDirection CurrentDirection { get; set; } = PlayerDirection.Right;

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
                this.PartiallyVisibleCells.AddIfNotNull(cell1);
                this.PartiallyVisibleCells.AddIfNotNull(cell2);

                currentCell = this.Maze.GetNextCell(currentCell, this.CurrentDirection);
            } while (currentCell != null);
        }

        private void OnKeyDown(KeyboardEventArgs e)
        {
            var direction = this.DirectionFromKey(e.Key);

            if (direction == this.CurrentDirection)
            {
                this.Move();
            } else if (direction is PlayerDirection directionToTurn)
            {
                this.Turn(directionToTurn);
            }
        }

        private PlayerDirection? DirectionFromKey(string key) =>
            key switch
            {
                ArrowLeft => PlayerDirection.Left,
                ArrowRight => PlayerDirection.Right,
                ArrowUp => PlayerDirection.Up,
                ArrowDown => PlayerDirection.Down,
                _ => null
            };

        private void Move()
        {
            var nextCell = this.Maze.GetNextCell(this.CurrentCell, this.CurrentDirection);

            if (nextCell != null)
            {
                var (cell1, cell2) = this.Maze.GetOrthogonalCells(this.CurrentCell, this.CurrentDirection);

                this.VisibleCells.Remove(this.CurrentCell);
                this.PartiallyVisibleCells.RemoveIfNotNull(cell1);
                this.PartiallyVisibleCells.RemoveIfNotNull(cell2);

                this.CurrentCell = nextCell;
            }
        }

        private void Turn(PlayerDirection direction)
        {
            this.CurrentDirection = direction;
            this.RecalculateVisiblities();
        }
    }
}

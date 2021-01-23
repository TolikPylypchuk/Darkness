using System;
using System.Threading;
using System.Threading.Tasks;

using Darkness.Maze;
using Darkness.Settings;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor;

namespace Darkness.Pages
{
    public partial class MazePage : ComponentBase
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

        [Inject]
        private IDialogService Dialog { get; set; } = null!;

        private MazeCanvas? MazeCanvas { get; set; }
        private ElementReference MazeWrapper { get; set; }

        private bool IsLoaded { get; set; } = false;
        private CancellationTokenSource MazeGenerationTokenSource { get; set; } = new();

        private GameSettings Settings { get; set; } = null!;
        private GameMaze Maze { get; set; } = null!;

        private Timer? Timer { get; set; }
        private TimeSpan Time { get; set; }
        private string TimerText => this.Time.ToString(@"hh\:mm\:ss");

        public void CancelIfGeneratingMaze()
        {
            this.MazeGenerationTokenSource.Cancel();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
            {
                return;
            }

            this.Settings = await this.SettingsService.GetSettings();

            this.StateHasChanged();

            try
            {
                this.Maze = await this.MazeGenerator.CreateMaze(this.Settings, this.MazeGenerationTokenSource.Token);

                this.StartTimer();

                this.IsLoaded = true;
                this.StateHasChanged();

                await this.MazeWrapper.FocusAsync();
            } catch (TaskCanceledException)
            { }
        }

        private void StartTimer()
        {
            void IncrementTime()
            {
                this.Time += TimeSpan.FromSeconds(1);
                this.StateHasChanged();
            }

            this.Timer = new(_ => IncrementTime(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private async Task BackToMainMenu()
        {
            this.CancelIfGeneratingMaze();
            await this.OnBackToMainMenuCallback.InvokeAsync();
        }

        private async Task ShowInfoDialog()
        {
            var dialog = this.Dialog.Show<InfoDialog>("How to Play");
            await dialog.Result;
            await this.MazeWrapper.FocusAsync();
        }

        private async Task OnKeyDown(KeyboardEventArgs e)
        {
            if (this.MazeCanvas != null)
            {
                PlayerDirection? direction = e.Key switch
                {
                    ArrowLeft => PlayerDirection.Left,
                    ArrowRight => PlayerDirection.Right,
                    ArrowUp => PlayerDirection.Up,
                    ArrowDown => PlayerDirection.Down,
                    _ => null
                };

                if (direction is PlayerDirection directionToMove)
                {
                    await this.MazeCanvas.Move(directionToMove);
                }
            }
        }

        private async Task OnFinished()
        {
            this.Snackbar.Add("You won!", Severity.Success);

            if (this.Timer != null)
            {
                await this.Timer.DisposeAsync();
            }
        }
    }
}

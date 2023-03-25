namespace Darkness.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor;

public partial class MazePage : ComponentBase
{
    private const string ArrowLeft = "ArrowLeft";
    private const string ArrowRight = "ArrowRight";
    private const string ArrowUp = "ArrowUp";
    private const string ArrowDown = "ArrowDown";

    [Inject]
    public required INavigator Navigator { get; init; }

    [Inject]
    public required ISettingsService SettingsService { get; init; }

    [Inject]
    public required IMazeGenerator MazeGenerator { get; init; }

    [Inject]
    public required ISnackbar Snackbar { get; init; }

    [Inject]
    public required IDialogService Dialog { get; set; }

    private MazeCanvas? MazeCanvas { get; set; }
    private ElementReference PageContainer { get; set; }

    private bool IsLoaded { get; set; }
    private CancellationTokenSource MazeGenerationTokenSource { get; set; } = new();

    private GameSettings Settings { get; set; } = null!;
    private GameMaze Maze { get; set; } = null!;

    private Timer? Timer { get; set; }
    private TimeSpan Time { get; set; }
    private string TimerText => this.Time.ToString(@"hh\:mm\:ss");

    private void CancelIfGeneratingMaze() =>
        this.MazeGenerationTokenSource.Cancel();

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

            await this.PageContainer.FocusAsync();
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

        this.Timer = new Timer(_ => IncrementTime(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private async ValueTask BackToHomePage()
    {
        this.CancelIfGeneratingMaze();
        await this.Navigator.GoToHomePage();
    }

    private async Task ShowInfoDialog()
    {
        var dialog = await this.Dialog.ShowAsync<InfoDialog>("How to Play");
        await dialog.Result;
        await this.PageContainer.FocusAsync();
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (this.MazeCanvas is { } canvas)
        {
            PlayerDirection? direction = e.Key switch
            {
                ArrowLeft => PlayerDirection.Left,
                ArrowRight => PlayerDirection.Right,
                ArrowUp => PlayerDirection.Up,
                ArrowDown => PlayerDirection.Down,
                _ => null
            };

            if (direction is { } directionToMove)
            {
                await canvas.Move(directionToMove);
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

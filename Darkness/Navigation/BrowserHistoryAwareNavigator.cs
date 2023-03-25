namespace Darkness.Navigation;

using BrowserInterop;
using BrowserInterop.Extensions;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public class BrowserHistoryAwareNavigator : INavigator
{
    private const string HomePage = "/";
    private const string MazePage = "/maze";
    private const string SettingsPage = "/settings";

    private readonly NavigationManager navigationManager;
    private readonly IJSRuntime jsRuntime;

    private bool goToMainPageUsingBrowserHistory = false;

    public BrowserHistoryAwareNavigator(NavigationManager navigationManager, IJSRuntime jsRuntime)
    {
        this.navigationManager = navigationManager;
        this.jsRuntime = jsRuntime;
    }

    public async ValueTask GoToHomePage()
    {
        if (this.goToMainPageUsingBrowserHistory)
        {
            var history = await this.GetBrowserHistory();
            await history.Back();
        } else
        {
            this.navigationManager.NavigateTo(HomePage, replace: true);
        }

        this.goToMainPageUsingBrowserHistory = false;
    }

    public ValueTask GoToMazePage()
    {
        this.goToMainPageUsingBrowserHistory = true;
        this.navigationManager.NavigateTo(MazePage);
        return ValueTask.CompletedTask;
    }

    public ValueTask GoToSettingsPage()
    {
        this.goToMainPageUsingBrowserHistory = true;
        this.navigationManager.NavigateTo(SettingsPage);
        return ValueTask.CompletedTask;
    }

    private async ValueTask<WindowHistory> GetBrowserHistory()
    {
        var window = await this.jsRuntime.Window();
        return window.History;
    }
}

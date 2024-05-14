using BrowserInterop;
using BrowserInterop.Extensions;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Darkness.Navigation;

public class BrowserHistoryAwareNavigator(NavigationManager navigationManager, IJSRuntime jsRuntime) : INavigator
{
    private const string HomePage = "/";
    private const string MazePage = "/maze";
    private const string SettingsPage = "/settings";

    private readonly NavigationManager navigationManager = navigationManager;
    private readonly IJSRuntime jsRuntime = jsRuntime;

    private bool goToMainPageUsingBrowserHistory = false;

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

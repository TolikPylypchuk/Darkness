namespace Darkness.Navigation;

public interface INavigator
{
    public ValueTask GoToHomePage();

    public ValueTask GoToMazePage();

    public ValueTask GoToSettingsPage();
}

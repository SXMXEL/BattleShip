using UI;

namespace Managers
{
    public class PageManager
    {
        private readonly StartPage _startPage;
        private readonly GamePage _gamePage;
        private readonly SettingsPage _settingsPage;

        public PageManager(StartPage startPage, GamePage gamePage, SettingsPage settingsPage)
        {
            _startPage = startPage;
            _gamePage = gamePage;
            _settingsPage = settingsPage;
        }

        public void SetPageState(PageState pageState)
        {
            _startPage.gameObject.SetActive(pageState == PageState.StartPage);
            _settingsPage.gameObject.SetActive(pageState == PageState.SettingsPage);
            _gamePage.gameObject.SetActive(pageState == PageState.GamePage);
        }
    }
}
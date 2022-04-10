using AccSaber.UI.MenuButton;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using System;
using Zenject;

namespace AccSaber.Managers
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private MenuButton _menuButton;
        private readonly AccSaberMainFlowCoordinator _accSaberFlowCoordinator;

        public MenuButtonManager( AccSaberMainFlowCoordinator accSaberFlowCoordinator)
        {
            _accSaberFlowCoordinator = accSaberFlowCoordinator;
            _menuButton = new MenuButton("AccSaber", PresentFlowCoordinator);
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        private void PresentFlowCoordinator()
        {
            _accSaberFlowCoordinator.Show();
        }

        public void Dispose()
        {
            if (MenuButtons.IsSingletonAvailable && BSMLParser.IsSingletonAvailable)
            {
                MenuButtons.instance.UnregisterButton(_menuButton);
            }
        }
    }
}

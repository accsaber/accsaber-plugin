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
        private readonly MainFlowCoordinator _mainFlowCoordinator;
        private readonly AccSaberMainFlowCoordinator _accsaberFlowCoordinator;

        public MenuButtonManager(MainFlowCoordinator mainFlowCoordinator, AccSaberMainFlowCoordinator accsaberFlowCoordinator)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _accsaberFlowCoordinator = accsaberFlowCoordinator;
            _menuButton = new MenuButton("AccSaber", PresentFlowCoordinator);
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        private void PresentFlowCoordinator()
        {
            _mainFlowCoordinator.PresentFlowCoordinator(_accsaberFlowCoordinator);
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

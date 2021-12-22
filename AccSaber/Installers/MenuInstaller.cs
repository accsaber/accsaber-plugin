using AccSaber.Downloaders;
using AccSaber.Managers;
using AccSaber.UI.MenuButton;
using AccSaber.UI.MenuButton.ViewControllers;
using SiraUtil;
using Zenject;
using ASDownloader = AccSaber.Downloaders.AccSaberDownloader;

namespace AccSaber.Installers
{
    public class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            // Song Downloading
            Container.Bind<AccSaberDownloader>()
                .AsSingle();
            Container.Bind<BeatSaverDownloader>()
                .AsSingle();
            Container.BindInterfacesTo<MenuButtonManager>()
                .AsSingle();
            Container.Bind<RankedMapsView>()
                .FromNewComponentAsViewController()
                .AsSingle();
            Container.Bind<SelectedMapView>()
                .FromNewComponentAsViewController()
                .AsSingle();
            Container.Bind<AccSaberMainFlowCoordinator>()
                .FromNewComponentOnNewGameObject()
                .AsSingle();
        }
    }
}
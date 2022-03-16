using AccSaber.Data;
using AccSaber.Downloaders;
using AccSaber.Managers;
using AccSaber.Models;
using AccSaber.UI.Leaderboard;
using AccSaber.UI.MenuButton;
using AccSaber.UI.MenuButton.ViewControllers;
using AccSaber.UI.Panel;
using SiraUtil;
using Zenject;
using ASDownloader = AccSaber.Downloaders.AccSaberDownloader;

namespace AccSaber.Installers
{
    public class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            // Song Downloading UI
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
            
            // Leaderboard
            Container.BindInterfacesAndSelfTo<AccSaberLeaderboardViewController>()
                .FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<AccSaberPanelController>()
                .FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<AccSaberManager>()
                .AsSingle().NonLazy();
            
            
        }
    }
}
using AccSaber.Data;
using AccSaber.Downloaders;
using AccSaber.Interfaces;
using AccSaber.Managers;
using AccSaber.Models;
using AccSaber.Sources;
using AccSaber.UI.Leaderboard;
using AccSaber.UI.MenuButton;
using AccSaber.UI.MenuButton.ViewControllers;
using AccSaber.UI.Panel;
using AccSaber.Utils;
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
            
            // Sources
            Container.BindInterfacesTo<GlobalLeaderboardSource>().AsSingle();
            Container.BindInterfacesTo<AroundMeLeaderboardSource>().AsSingle();
            
            // Leaderboard
            Container.BindInterfacesAndSelfTo<AccSaberLeaderboardViewController>()
                .FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<AccSaberPanelViewController>()
                .FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<AccSaberManager>()
                .AsSingle();
        }
    }
}
using AccSaber.Downloaders;
using AccSaber.HarmonyPatches;
using AccSaber.Managers;
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
            // Leaderboard
            Container.BindInterfacesAndSelfTo<AccSaberManager>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<AccSaberLeaderboardViewController>()
                .FromNewComponentAsViewController()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<AccSaberPanelController>()
                .FromNewComponentAsViewController()
                .AsSingle();
            
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
                .FromNewComponentOnNewGameObject(nameof(AccSaberMainFlowCoordinator))
                .AsSingle();
            if (Plugin.playlistManagerPatching)
            {
                Container.Bind<PlaylistManagerPatcher>()
                    .AsSingle()
                    .NonLazy();
            }
        }
    }
}
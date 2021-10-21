using AccSaber.Downloaders;
using AccSaber.HarmonyPatches;
using AccSaber.Managers;
using AccSaber.UI.MenuButton;
using AccSaber.UI.MenuButton.ViewControllers;
using SiraUtil;
using Zenject;

namespace AccSaber.Installers
{
    public class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<AccSaberDownloader>().AsSingle();
            Container.Bind<BeatSaverDownloader>().AsSingle();
            Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
            Container.Bind<RankedMapsView>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<SelectedMapView>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<AccSaberMainFlowCoordinator>().FromNewComponentOnNewGameObject(nameof(AccSaberMainFlowCoordinator)).AsSingle();
            if (Plugin.playlistManagerPatching)
            {
                Container.Bind<PlaylistManagerPatcher>().AsSingle().NonLazy();
            }
        }
    }
}
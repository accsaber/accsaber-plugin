using AccSaber.UI.Leaderboard;
using AccSaber.UI.Leaderboard.Panel;
using SiraUtil;
using Zenject;

namespace AccSaber.Installers
{
    public class MenuInstaller : Installer
    {
        public MenuInstaller()
        {
        }
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AccSaberLeaderboardViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<AccSaberPanelController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<LeaderboardUI>().AsSingle();
        }
    }
}
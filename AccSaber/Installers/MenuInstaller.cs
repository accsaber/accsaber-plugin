using AccSaber.UI.Leaderboard;
using AccSaber.UI.Leaderboard.Panel;
using AccSaber.Managers;
using SiraUtil;
using SiraUtil.Tools;
using Zenject;

namespace AccSaber.Installers
{
    public class MenuInstaller : Installer
    {
        private readonly SiraLog _log;
        public MenuInstaller(SiraLog log)
        {
            _log = log;
        }
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AccSaberLeaderboardViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<AccSaberPanelController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<AccSaberManager>().AsSingle().NonLazy();
        }
    }
}
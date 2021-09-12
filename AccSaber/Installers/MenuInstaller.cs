using AccSaber.UI.Leaderboard;
using AccSaber.Managers;
using AccSaber.UI.Panel;
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
            Container.BindInterfacesAndSelfTo<AccSaberLeaderboardViewController>().FromNewComponentAsViewController().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AccSaberPanelController>().FromNewComponentAsViewController().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AccSaberManager>().AsSingle().NonLazy();
        }
    }
}
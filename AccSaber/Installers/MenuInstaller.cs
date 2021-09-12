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
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AccSaberLeaderboardViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<AccSaberPanelController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<AccSaberManager>().AsSingle().NonLazy();
        }
    }
}
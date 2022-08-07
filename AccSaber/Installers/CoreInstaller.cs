using AccSaber.Data;
using AccSaber.Downloaders;
using AccSaber.Models;
using AccSaber.Utils;
using Zenject;

namespace AccSaber.Installers
{
    internal class CoreInstaller : Installer
    {
        public override void InstallBindings()
        {
            // Song Downloading
            Container.Bind<AccSaberDownloader>()
                .AsSingle();
            Container.Bind<BeatSaverDownloader>()
                .AsSingle();
            Container.Bind<LeaderboardDownloader>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<UserInfoDownloader>()
                .AsSingle();

            // Utilities
            Container.Bind<GameUtils>().AsSingle();
            Container.BindInterfacesAndSelfTo<UserIDUtils>().AsSingle();

            // Data
            Container.BindInterfacesAndSelfTo<AccSaberData>().AsSingle();
            Container.Bind<AccSaberAPISong>().AsSingle();
            Container.Bind<AccSaberCategory>().AsSingle();
            Container.Bind<AccSaberLeaderboardEntry>().AsSingle();
            Container.Bind<AccSaberSong>().AsSingle();
            Container.Bind<AccSaberUserModel>().AsSingle();
            Container.Bind<AccSaberSongDiff>().AsSingle();
        }
    }
}
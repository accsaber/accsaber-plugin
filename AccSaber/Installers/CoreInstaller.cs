using AccSaber.Data;
using AccSaber.Downloaders;
using AccSaber.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            // Data
            Container.BindInterfacesAndSelfTo<AccSaberData>().AsSingle();
            Container.Bind<AccSaberAPISong>().AsSingle();
            Container.Bind<AccSaberCategory>().AsSingle();
            Container.Bind<AccSaberLeaderboardEntries>().AsSingle();
            Container.Bind<AccSaberSong>().AsSingle();
            Container.Bind<AccSaberSongDiff>().AsSingle();
        }
    }
}

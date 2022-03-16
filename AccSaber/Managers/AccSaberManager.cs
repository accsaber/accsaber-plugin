using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AccSaber.UI.Leaderboard;
using AccSaber.UI.Panel;
using HMUI;
using AccSaber.Downloaders;
using LeaderboardCore.Managers;
using LeaderboardCore.Models;
using Zenject;
using AccSaber.Data;
using AccSaber.Models;
using SiraUtil.Logging;
using static StandardLevelDetailViewController.ContentType;

namespace AccSaber.Managers
{
    class AccSaberManager : CustomLeaderboard, IInitializable, IDisposable
    {
        private readonly CustomLeaderboardManager _customLeaderboardManager;

        private readonly ViewController _accSaberPanelController;
        protected override ViewController panelViewController => _accSaberPanelController;

        private readonly AccSaberLeaderboardViewController _mainLeaderboardViewController;
        protected override ViewController leaderboardViewController => _mainLeaderboardViewController;

        private readonly LevelCollectionNavigationController _navigationController;

        private AccSaberData _accSaberData;
        private AccSaberDownloader _downloader;
        private readonly AccSaberAPISong _accSaberAPISong;
        private readonly AccSaberLeaderboardEntries _leaderboardEntries;

        private readonly SiraLog _log;

        public AccSaberManager(AccSaberPanelController accSaberPanelController,
            AccSaberLeaderboardViewController mainLeaderboardViewController,
            CustomLeaderboardManager customLeaderboardManager,
            SiraLog log,
            LevelCollectionNavigationController navigationController,
            AccSaberData accSaberData,
            AccSaberDownloader downloader,
            AccSaberAPISong accSaberAPISong, AccSaberLeaderboardEntries leaderboardEntries)
        {
            _customLeaderboardManager = customLeaderboardManager;
            _log = log;
            _mainLeaderboardViewController = mainLeaderboardViewController;
            _accSaberPanelController = accSaberPanelController;
            _navigationController = navigationController;
            _accSaberData = accSaberData;
            _downloader = downloader;
            _accSaberAPISong = accSaberAPISong;
            _leaderboardEntries = leaderboardEntries;
        }

        public void Initialize()
        {
            _navigationController.didChangeLevelDetailContentEvent += OnSongChange;
            _navigationController.didChangeDifficultyBeatmapEvent += OnBeatmapChange;
        }

        public void Dispose()
        {
            _navigationController.didChangeLevelDetailContentEvent -= OnSongChange;
            _navigationController.didChangeDifficultyBeatmapEvent -= OnBeatmapChange;
        }

        private void OnBeatmapChange(LevelCollectionNavigationController collectionNavigationController,
            IDifficultyBeatmap difficultyBeatmap)
        {
            _log.Info("Registering leaderboard..");
            HandleRankedSongLeaderboard(_navigationController);
        }

        private void OnSongChange(LevelCollectionNavigationController collectionNavigationController, 
            StandardLevelDetailViewController.ContentType contentType)
        {
            if (contentType == OwnedAndReady) HandleRankedSongLeaderboard(collectionNavigationController);
        }
        

        private void HandleRankedSongLeaderboard(LevelCollectionNavigationController collectionNavigationController)
        {
            var beatmap = collectionNavigationController.selectedDifficultyBeatmap;
            
            try
            { 
                if (beatmap == null) return;

                _log.Info("Is data initialized?");
                _log.Info($"{_accSaberData.IsDataInitialized}");

                if (!_accSaberData.IsDataInitialized)
                {
                    return;
                }
                foreach (var rankedSong in _accSaberData.GetMapsFromHash(Extensions.GetRankedSongHash(beatmap.level.levelID)))
                {
                    if (string.Equals(beatmap.difficulty.ToString(), rankedSong.difficulty, StringComparison.CurrentCultureIgnoreCase))
                    {
                        _customLeaderboardManager.Register(this);
                        return;
                    }
                }

                _log.Debug("Unregistering leaderboard..");
                _customLeaderboardManager.Unregister(this);
            }
            catch (Exception e)
            {

                _log.Warn($"Message: {e}");
                _log.Warn($"Value: {collectionNavigationController.selectedBeatmapLevel?.levelID.Substring(13)}");
                _log.Warn($"Value: {_accSaberAPISong.songHash}");
            }
        }
    }
}

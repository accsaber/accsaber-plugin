using System;
using System.Collections.Generic;
using System.Threading;
using AccSaber.UI.Leaderboard;
using AccSaber.UI.Panel;
using HMUI;
using AccSaber.Downloaders;
using AccSaber.Utils;
using LeaderboardCore.Managers;
using LeaderboardCore.Models;
using SiraUtil.Tools;
using Zenject;

namespace AccSaber.Managers
{
    class AccSaberManager : CustomLeaderboard, IInitializable, IDisposable
    {
        private CustomLeaderboardManager _customLeaderboardManager;
        
        private readonly ViewController _accSaberPanelController;
        protected override ViewController panelViewController => _accSaberPanelController;
        
        private readonly ViewController _mainLeaderboardViewController;
        protected override ViewController leaderboardViewController => _mainLeaderboardViewController;
        
        private LevelCollectionNavigationController _navigationController;
        private AccSaberDownloader _accSaberDownloader;
        private List<AccSaberUtils.AccSaberAPISong> _apiSongList;

        internal static CancellationTokenSource closeCancellationTokenSource;

        [Inject]
        private SiraLog _log;
        
        public AccSaberManager(AccSaberPanelController accSaberPanelController,
            AccSaberLeaderboardViewController mainLeaderboardViewController, 
            CustomLeaderboardManager customLeaderboardManager, SiraLog log,
            LevelCollectionNavigationController navigationController,
            AccSaberDownloader accSaberDownloader)
        {
            _customLeaderboardManager = customLeaderboardManager;
            _log = log;
            _mainLeaderboardViewController = mainLeaderboardViewController;
            _accSaberPanelController = accSaberPanelController;
            _navigationController = navigationController;
            _accSaberDownloader = accSaberDownloader;
        }

        public void Initialize()
        {
            _navigationController.didChangeLevelDetailContentEvent += OnSongChange;
            _navigationController.didChangeDifficultyBeatmapEvent += OnBeatmapChange;
            _log.Debug("Getting Ranked maps..");
            GetRankedMaps();
            _log.Debug("Registering Leaderboard..");
            RegisterRankedSongLeaderboard(_navigationController);
        }

        public void Dispose()
        {
            _navigationController.didChangeLevelDetailContentEvent -= OnSongChange;
            _navigationController.didChangeDifficultyBeatmapEvent -= OnBeatmapChange;
            closeCancellationTokenSource?.Cancel();
        }

        private async void GetRankedMaps()
        {
            closeCancellationTokenSource = new CancellationTokenSource();
            _apiSongList = await _accSaberDownloader.GetRankedMapsAsync(closeCancellationTokenSource.Token);
        }

        private void OnBeatmapChange(LevelCollectionNavigationController collectionNavigationController, IDifficultyBeatmap difficultyBeatmap)
        {
            _log.Info("registering leaderboard.. (OnBeatmapChange)");
            RegisterRankedSongLeaderboard(_navigationController);
        }

        private void RegisterRankedSongLeaderboard(LevelCollectionNavigationController collectionNavigationController)
        {
            if (collectionNavigationController.selectedDifficultyBeatmap == null) return;
            foreach (var RankedSong in _apiSongList)
            {
                _log.Info("checking hashes..");
                if (GetRankedSongHash(collectionNavigationController.selectedBeatmapLevel.levelID) ==
                    RankedSong.songHash && String.Equals(collectionNavigationController.selectedDifficultyBeatmap.difficulty.ToString(),
                        RankedSong.difficulty, StringComparison.CurrentCultureIgnoreCase))
                {
                    _log.Info("registering leaderboard.. (RegisterRankedSongLeaderboard)");
                    _customLeaderboardManager.Register(this);
                    return;
                }
            }
            _customLeaderboardManager.Unregister(this);
        }

        private static string GetRankedSongHash(string levelId)
        {
            return !levelId.Contains("custom_level_") ? levelId : levelId.Substring(13);
        }

        private void OnSongChange(LevelCollectionNavigationController collectionController, StandardLevelDetailViewController.ContentType contentType)
        {
            if (contentType == StandardLevelDetailViewController.ContentType.OwnedAndReady) RegisterRankedSongLeaderboard(collectionController);
        }
    }
}
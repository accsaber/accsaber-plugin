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
using AccSaber.Data;

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
        private AccSaberData _accSaberData;

        [Inject]
        private SiraLog _log;
        
        public AccSaberManager(AccSaberPanelController accSaberPanelController,
            AccSaberLeaderboardViewController mainLeaderboardViewController, 
            CustomLeaderboardManager customLeaderboardManager, SiraLog log,
            LevelCollectionNavigationController navigationController,
            AccSaberData accSaberData)
        {
            _customLeaderboardManager = customLeaderboardManager;
            _log = log;
            _mainLeaderboardViewController = mainLeaderboardViewController;
            _accSaberPanelController = accSaberPanelController;
            _navigationController = navigationController;
            _accSaberData = accSaberData;
        }

        public void Initialize()
        {
            _navigationController.didChangeLevelDetailContentEvent += OnSongChange;
            _navigationController.didChangeDifficultyBeatmapEvent += OnBeatmapChange;
            RegisterRankedSongLeaderboard(_navigationController);
        }

        public void Dispose()
        {
            _navigationController.didChangeLevelDetailContentEvent -= OnSongChange;
            _navigationController.didChangeDifficultyBeatmapEvent -= OnBeatmapChange;
        }

        private void OnBeatmapChange(LevelCollectionNavigationController collectionNavigationController, IDifficultyBeatmap difficultyBeatmap)
        {
            _log.Info("Registering leaderboard..");
            RegisterRankedSongLeaderboard(_navigationController);
        }

        private void RegisterRankedSongLeaderboard(LevelCollectionNavigationController collectionNavigationController)
        {
            if (collectionNavigationController.selectedDifficultyBeatmap == null)
            {
                return;
            }

            foreach (var rankedSong in _accSaberData.GetMapsFromHash(GetRankedSongHash(collectionNavigationController.selectedBeatmapLevel.levelID)))
            {
                if (String.Equals(collectionNavigationController.selectedDifficultyBeatmap.difficulty.ToString(), rankedSong.difficulty, StringComparison.CurrentCultureIgnoreCase))
                {
                    _log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Registering leaderboard..");
                    _customLeaderboardManager.Register(this);
                    return;
                }
            }
            _log.Debug("Unregistering leaderboard..");
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
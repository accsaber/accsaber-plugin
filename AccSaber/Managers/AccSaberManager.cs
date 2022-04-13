using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.UI.Leaderboard;
using AccSaber.UI.Panel;
using HMUI;
using AccSaber.Downloaders;
using LeaderboardCore.Managers;
using LeaderboardCore.Models;
using Zenject;
using AccSaber.Data;
using AccSaber.Interfaces;
using AccSaber.Models;
using AccSaber.Sources;
using AccSaber.Utils;
using JetBrains.Annotations;
using LeaderboardCore.Interfaces;
using SiraUtil.Logging;
using static StandardLevelDetailViewController.ContentType;

namespace AccSaber.Managers
{
    class AccSaberManager : CustomLeaderboard, IInitializable, IDisposable, INotifyLeaderboardSet
    {
        private readonly CustomLeaderboardManager _customLeaderboardManager;

        private readonly AccSaberPanelViewController _accSaberPanelController;
        protected override ViewController panelViewController => _accSaberPanelController;

        private readonly AccSaberLeaderboardViewController _mainLeaderboardViewController;
        protected override ViewController leaderboardViewController => _mainLeaderboardViewController;

        private readonly LevelCollectionNavigationController _navigationController;

        private AccSaberDownloader _accSaberDownloader;
        private readonly AccSaberData _accSaberData;
        private readonly List<AccSaberAPISong> _accSaberAPISong;
        private List<AccSaberLeaderboardEntry> _leaderboardEntries;
        private AccSaberPanelViewController _panelViewController;

        private readonly List<IDifficultyBeatmapUpdater> difficultyBeatmapUpdaters;

        private readonly AccSaberLeaderboardViewController _accSaberLeaderboardViewController;
        private readonly AccSaberPanelViewController _accSaberPanelViewController;

        private readonly List<INotifyViewActivated> _notifyViewActivateds;

        private ILeaderboardSource _leaderboardSource;
        private LeaderboardDownloader _leaderboardDownloader;
        private IDifficultyBeatmap selectedDifficultyBeatmap;

        private readonly SiraLog _log;
        private CancellationTokenSource levelInfoTokenSource;
        private CancellationTokenSource leaderboardTokenSource;
        private readonly CancellationTokenSource _cancellationToken;

        public int page;
        private LevelCollectionNavigationController beatmap;

        public AccSaberManager(AccSaberPanelViewController accSaberPanelController,
            AccSaberLeaderboardViewController mainLeaderboardViewController,
            CustomLeaderboardManager customLeaderboardManager,
            SiraLog log,
            LevelCollectionNavigationController navigationController,
            AccSaberData accSaberData,
            AccSaberDownloader accSaberDownloader,
            List<AccSaberAPISong> accSaberAPISong,
            List<AccSaberLeaderboardEntry> leaderboardEntries,
            List<INotifyViewActivated> notifyViewActivateds,
            AccSaberLeaderboardViewController accSaberLeaderboardViewController,
            LevelCollectionNavigationController beatmap,
            AccSaberPanelViewController panelViewController,
            LeaderboardDownloader leaderboardDownloader)
        {
            _customLeaderboardManager = customLeaderboardManager;
            _log = log;
            _mainLeaderboardViewController = mainLeaderboardViewController;
            _accSaberPanelController = accSaberPanelController;
            _navigationController = navigationController;
            _accSaberData = accSaberData;
            _accSaberDownloader = accSaberDownloader;
            _accSaberAPISong = accSaberAPISong;
            _leaderboardEntries = leaderboardEntries;
            _notifyViewActivateds = notifyViewActivateds;
            _accSaberLeaderboardViewController = accSaberLeaderboardViewController;
            this.beatmap = beatmap;
            _panelViewController = panelViewController;
            _leaderboardDownloader = leaderboardDownloader;
        }

        public void Initialize()
        {
            _navigationController.didChangeLevelDetailContentEvent += OnSongChange;
            _navigationController.didChangeDifficultyBeatmapEvent += OnBeatmapChange;

            _accSaberLeaderboardViewController.didActivateEvent += OnViewActivated;
            // _accSaberLeaderboardViewController.PageRequested += OnPageRequested;
        }

        public void Dispose()
        {
            _navigationController.didChangeLevelDetailContentEvent -= OnSongChange;
            _navigationController.didChangeDifficultyBeatmapEvent -= OnBeatmapChange;


            _accSaberLeaderboardViewController.didActivateEvent -= OnViewActivated;
            // _accSaberLeaderboardViewController.PageRequested -= OnPageRequested;
        }


        private void OnBeatmapChange(LevelCollectionNavigationController collectionNavigationController,
            IDifficultyBeatmap difficultyBeatmap)
        {
            _log.Info("Registering leaderboard..");
            HandleRankedSongLeaderboard(_navigationController);
            HandleCategoryColorChange();
        }

        private void OnSongChange(LevelCollectionNavigationController collectionNavigationController,
            StandardLevelDetailViewController.ContentType contentType)
        {
            if (contentType == OwnedAndReady)
            {
                HandleRankedSongLeaderboard(collectionNavigationController);
                HandleCategoryColorChange();
            }
        }

        private void HandleCategoryColorChange()
        {
            _panelViewController.RefreshAndSkewBannerColor();
        }


        private void HandleRankedSongLeaderboard(LevelCollectionNavigationController collectionNavigationController)
        {
            var difficultyBeatmap = collectionNavigationController.selectedDifficultyBeatmap;

            if (difficultyBeatmap == null) return;

            if (!_accSaberData.IsMapDataInitialized)
            {
                return;
            }

            if (_accSaberData.GetMapsFromHash(difficultyBeatmap.level.levelID.GetRankedSongHash()).Any(x =>
                    x.difficulty.Equals(difficultyBeatmap.difficulty.ToString(), StringComparison.InvariantCultureIgnoreCase)))
            {
                _customLeaderboardManager.Register(this);
            }
            else
            {
                _customLeaderboardManager.Unregister(this);
            }
        }

        public void OnLeaderboardSet(IDifficultyBeatmap difficultyBeatmap) => _ = OnLeaderboardSetAsync(difficultyBeatmap);

        private async Task OnLeaderboardSetAsync(IDifficultyBeatmap difficultyBeatmap)
        {
            if (difficultyBeatmap != null)
            {
                selectedDifficultyBeatmap = difficultyBeatmap;
                // levelInfoTokenSource.Cancel();
                // levelInfoTokenSource.Dispose();

                if (difficultyBeatmap.level is CustomPreviewBeatmapLevel)
                {
                    levelInfoTokenSource = new CancellationTokenSource();
                    var accSaberLeaderboardEntries = await _leaderboardDownloader.GetLeaderboardAsync(difficultyBeatmap, levelInfoTokenSource.Token);
                    await IPA.Utilities.Async.UnityMainThreadTaskScheduler.Factory.StartNew(() => _accSaberLeaderboardViewController.LeaderboardEntriesUpdated(accSaberLeaderboardEntries));
                }
            }
        }

        // private void OnPageRequested(GameUtils beatmapUtils, int pageParams, int pageSize,
        //     CancellationToken source) =>
        //     _ = OnPageRequestedAsync(_leaderboardEntries, _leaderboardSource, pageParams);
        //
        // private async Task OnPageRequestedAsync(List<AccSaberLeaderboardEntries> leaderboardEntries, ILeaderboardSource leaderboardSource, int page)
        // {
        //     _log.Info("Hello..?");
        //     
        //     leaderboardTokenSource?.Cancel();
        //     leaderboardTokenSource?.Dispose();
        //     leaderboardTokenSource = new CancellationTokenSource();
        //     
        //     var leaderboardEntriesList = await leaderboardSource
        //         .AccSaberDownloader
        //         .GetLeaderboardsAsync(beatmap.selectedDifficultyBeatmap.level.levelID.GetRankedSongHash(),
        //             beatmap.selectedDifficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName,
        //             beatmap.selectedDifficultyBeatmap.difficulty.SerializedName(),
        //             page, 
        //             10,
        //             leaderboardTokenSource.Token);
        //     
        //     if (leaderboardEntriesList != null)
        //     {
        //         if (leaderboardEntriesList.Count == 0 || leaderboardEntriesList[0].ap == 0)
        //         {
        //             leaderboardEntriesList = null;
        //         }
        //     }
        // }


        private void OnViewActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            foreach (var notifyViewActivated in _notifyViewActivateds)
            {
                notifyViewActivated.ViewActivated(_accSaberLeaderboardViewController, firstActivation, addedToHierarchy, screenSystemEnabling);
            }
        }
    }
}
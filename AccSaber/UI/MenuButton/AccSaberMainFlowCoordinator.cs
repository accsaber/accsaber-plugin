using AccSaber.Downloaders;
using AccSaber.UI.MenuButton.ViewControllers;
using AccSaber.Utils;
using BeatSaberMarkupLanguage;
using HMUI;
using SiraUtil.Tools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zenject;
using static AccSaber.Utils.AccSaberUtils;

namespace AccSaber.UI.MenuButton
{
    class AccSaberMainFlowCoordinator : FlowCoordinator
    {
        private SiraLog _siraLog;
        private AccSaberDownloader _accSaberDownloader;
        private BeatSaverDownloader _beatSaverDownloader;

        private MainFlowCoordinator _mainFlowCoordinator;
        private RankedMapsView _rankedMapsView;
        private static SelectedMapView _selectedMapView;

        internal static CancellationTokenSource closeCancellationTokenSource;

        [Inject]
        protected void Construct(SiraLog siraLog, MainFlowCoordinator mainFlowCoordinator, RankedMapsView rankedMapsView, SelectedMapView selectedMapView, AccSaberDownloader accSaberDownloader, BeatSaverDownloader beatSaverDownloader)
        {
            _siraLog = siraLog;

            _mainFlowCoordinator = mainFlowCoordinator;
            _rankedMapsView = rankedMapsView;
            _selectedMapView = selectedMapView;

            _accSaberDownloader = accSaberDownloader;
            _beatSaverDownloader = beatSaverDownloader;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            closeCancellationTokenSource = new CancellationTokenSource();
            if (firstActivation)
            {
                SetTitle("AccSaber Map Downloader");
                showBackButton = true;
                ProvideInitialViewControllers(_rankedMapsView, null, _selectedMapView);

                SongCore.Loader.SongsLoadedEvent += SongcoreSongsLoaded;
                FetchRankedMaps();
                FetchAccSaberCategories();
            }

            else
            {
                // fetching might have been cancelled
                if (_rankedMapsView.rankedSongs == null || _rankedMapsView.rankedSongs.Count == 0)
                {
                    FetchRankedMaps();
                }

                if (RankedMapsView.filteringOptions == null | RankedMapsView.filteringOptions.Count < 6)
                {
                    FetchAccSaberCategories();
                }
            }

            _rankedMapsView.RefreshSongs();

            if (addedToHierarchy)
            {

            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            if (IsDownloading())
            {
                _rankedMapsView.ShowCancelConfirmation();
            }
            else
            {
                ForceClose();
            }
        }

        internal void ForceClose()
        {
            closeCancellationTokenSource?.Cancel();
            closeCancellationTokenSource = null;
            SelectedMapView.coverCancel?.Cancel();
            _mainFlowCoordinator.DismissFlowCoordinator(this);
        }

        private void SongcoreSongsLoaded(object arg1, object arg2)
        {
            foreach (var map in _rankedMapsView.rankedSongs)
            {
                map.IsDownloaded();
            }

            _rankedMapsView.SongCoreReady();
            _selectedMapView.SongCoreReady();
        }

        internal static void SetSelectedSong(AccSaberSong song)
        {
            _selectedMapView.SetSelectedSong(song);
        }

        private async void FetchRankedMaps()
        {
            List<AccSaberAPISong> rankedMaps = await _accSaberDownloader.GetRankedMapsAsync(closeCancellationTokenSource.Token);
            var songs = CreateAccSaberSongs(rankedMaps);
            _rankedMapsView.SetRankedMaps(songs);
        }

        private async void FetchAccSaberCategories()
        {
            List<AccSaberCategory> categories = await _accSaberDownloader.GetCategoriesAsync(closeCancellationTokenSource.Token);
            foreach (var category in categories)
            {
                AccSaberUtils.SetKnownCategory(category);
            }
            _rankedMapsView.SetFilters(categories);
        }

        private List<AccSaberSongBSML> CreateAccSaberSongs(List<AccSaberAPISong> rankedMaps)
        {
            Dictionary<string, AccSaberSongBSML> songsByHash = new Dictionary<string, AccSaberSongBSML>();
            List<AccSaberSongBSML> songs = new List<AccSaberSongBSML>();

            foreach (var apiSong in rankedMaps)
            {
                if (songsByHash.ContainsKey(apiSong.songHash))
                {
                    var accSaberSong = songsByHash[apiSong.songHash];
                    var diff = new AccSaberSongDiff(apiSong.categoryDisplayName, apiSong.difficulty, apiSong.complexity);
                    accSaberSong.AddDiff(diff);
                }

                else
                {
                    var diff = new AccSaberSongDiff(apiSong.categoryDisplayName, apiSong.difficulty, apiSong.complexity);
                    var diffs = new List<AccSaberSongDiff>() { diff };
                    var accSaberSong = new AccSaberSongBSML(apiSong.songName, apiSong.songSubName, apiSong.songAuthorName, apiSong.levelAuthorName, apiSong.beatSaverKey, apiSong.songHash, diffs);
                    songsByHash.Add(apiSong.songHash, accSaberSong);
                    songs.Add(accSaberSong);
                }
            }

            return songs;
        }

        internal async Task<bool> DownloadSong(AccSaberSong song, Action<float> progressCallback, Action<string> statusCallback)
        {
            return await _beatSaverDownloader.DownloadOldVersionByHash(song.songHash, closeCancellationTokenSource.Token, song, progressCallback, statusCallback);
        }

        internal bool IsDownloading()
        {
            return _rankedMapsView.IsDownloading();
        }

        internal void StartDownloading(bool multiDownload, AccSaberSong song = null)
        {
            if (multiDownload)
            {
                _selectedMapView.MultiDownloadStarted();
            }
            else
            {
                _rankedMapsView.DownloadSongInternal(song);
            }
        }
    }
}

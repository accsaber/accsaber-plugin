using AccSaber.Downloaders;
using AccSaber.UI.MenuButton.ViewControllers;
using AccSaber.Utils;
using BeatSaberMarkupLanguage;
using HMUI;
using SiraUtil.Tools;
using System;
using System.Collections.Generic;
using Zenject;
using static AccSaber.Utils.AccSaberUtils;

namespace AccSaber.UI.MenuButton
{
    class AccSaberMainFlowCoordinator : FlowCoordinator
    {
        private SiraLog _siraLog;
        private AccSaberDownloader _accsaberDownloader;

        private MainFlowCoordinator _mainFlowCoordinator;
        private RankedMapsView _rankedMapsView;
        private static SelectedMapView _selectedMapView;

        [Inject]
        protected void Construct(SiraLog siraLog, MainFlowCoordinator mainFlowCoordinator, RankedMapsView rankedMapsView, SelectedMapView selectedMapView, AccSaberDownloader accSaberDownloader)
        {
            _siraLog = siraLog;

            _mainFlowCoordinator = mainFlowCoordinator;
            _rankedMapsView = rankedMapsView;
            _selectedMapView = selectedMapView;

            _accsaberDownloader = accSaberDownloader;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                SetTitle("AccSaber");
                showBackButton = true;
                ProvideInitialViewControllers(_rankedMapsView, null, _selectedMapView);

                SongCore.Loader.SongsLoadedEvent += SongcoreSongsLoaded;
                FetchRankedMaps();
                FetchAccSaberCategories();
            }

            if (addedToHierarchy)
            {

            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
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
            List<AccSaberAPISong> rankedMaps = await _accsaberDownloader.GetRankedMapsAsync();
            var songs = CreateAccSaberSongs(rankedMaps);
            _siraLog.Debug(songs.Count);
            _rankedMapsView.SetRankedMaps(songs);
        }

        private async void FetchAccSaberCategories()
        {
            List<AccSaberCategory> categories = await _accsaberDownloader.GetCategoriesAsync();
            foreach (var category in categories)
            {
                AccSaberUtils.SetKnownCategory(category);
            }
            _rankedMapsView.SetFilters(categories);
        }

        private List<AccSaberSong> CreateAccSaberSongs(List<AccSaberAPISong> rankedMaps)
        {
            Dictionary<string, AccSaberSong> songsByHash = new Dictionary<string, AccSaberSong>();
            List<AccSaberSong> songs = new List<AccSaberSong>();

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
                    //var cover = await _accsaberDownloader.GetCoverImageAsync("B70AEEF2EE915CED48593422931E8BA2A1F4E973");
                    var accSaberSong = new AccSaberSong(apiSong.songName, apiSong.songSubName, apiSong.songAuthorName, apiSong.levelAuthorName, apiSong.beatSaverKey, apiSong.songHash, diffs);
                    songsByHash.Add(apiSong.songHash, accSaberSong);
                    songs.Add(accSaberSong);
                }
            }

            return songs;
        }
    }
}

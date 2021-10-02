using AccSaber.Downloaders;
using BeatSaberPlaylistsLib.Types;
using IPA.Utilities;
using PlaylistManager.UI;
using SiraUtil.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace AccSaber.HarmonyPatches
{
    class PlaylistManagerPatcher
    {
        private BeatSaverDownloader _beatSaverDownloader;
        private SiraLog _siraLog;

        private IAnnotatedBeatmapLevelCollection[] _downloadingBeatmapLevelCollections;
        private int _downloadingBeatmapCollectionIdx;
        private PlaylistViewButtonsController _playlistViewButtonsController;

        public PlaylistManagerPatcher(BeatSaverDownloader beatSaverDownloader, SiraLog siraLog)
        {
            _beatSaverDownloader = beatSaverDownloader;
            _siraLog = siraLog;
            DownloadPlaylistAsyncPatch.downloadMissingSongsCallback += DownloadMissingSongs;
        }

        private async void DownloadMissingSongs(PlaylistViewButtonsController playlistViewButtonsController)
        {
            _playlistViewButtonsController = playlistViewButtonsController;

            var popupModalsController = playlistViewButtonsController.GetField<PopupModalsController, PlaylistViewButtonsController>("popupModalsController");

            var tokenSource = playlistViewButtonsController.GetField<CancellationTokenSource, PlaylistViewButtonsController>("tokenSource");
            tokenSource?.Dispose();
            tokenSource = new CancellationTokenSource();

            var rootTransform = playlistViewButtonsController.GetField<Transform, PlaylistViewButtonsController>("rootTransform");

            PopupModalsController.ButtonPressed cancel = () => tokenSource.Cancel();
            popupModalsController.InvokeMethod<object, PopupModalsController>("ShowOkModal", rootTransform, "", cancel, "Cancel", true);

            var i = 0;
            var missingSongs = playlistViewButtonsController.GetProperty<List<IPlaylistSong>, PlaylistViewButtonsController>("MissingSongs");
            popupModalsController.SetProperty<PopupModalsController, string>("OkText", string.Format("{0}/{1} songs downloaded", i, missingSongs.Count));

            foreach (var song in missingSongs)
            {
                await _beatSaverDownloader.DownloadOldVersionByHash(song.Hash, tokenSource.Token);
                i++;
                popupModalsController.SetProperty<PopupModalsController, string>("OkText", string.Format("{0}/{1} songs downloaded", i, missingSongs.Count));
            }

            popupModalsController.SetProperty<PopupModalsController, string>("OkText", "Download Complete!");
            popupModalsController.SetProperty<PopupModalsController, string>("OkButtonText", "Ok");

            var annotatedBeatmapLevelCollectionsViewController = playlistViewButtonsController.GetField<AnnotatedBeatmapLevelCollectionsViewController, PlaylistViewButtonsController>("annotatedBeatmapLevelCollectionsViewController");
            _downloadingBeatmapLevelCollections = annotatedBeatmapLevelCollectionsViewController.GetField<IReadOnlyList<IAnnotatedBeatmapLevelCollection>, AnnotatedBeatmapLevelCollectionsViewController>("_annotatedBeatmapLevelCollections").ToArray();
            _downloadingBeatmapCollectionIdx = annotatedBeatmapLevelCollectionsViewController.selectedItemIndex;
            UpdateSecondChildControllerContentPatch.SecondChildControllerUpdatedEvent += LevelFilteringNavigationController_UpdateSecondChildControllerContent_SecondChildControllerUpdatedEvent;
            SongCore.Loader.Instance.RefreshSongs(false);
        }

        private void LevelFilteringNavigationController_UpdateSecondChildControllerContent_SecondChildControllerUpdatedEvent()
        {
            Action<IAnnotatedBeatmapLevelCollection[], int> LevelCollectionTableViewUpdatedEvent = _playlistViewButtonsController.GetField<Action<IAnnotatedBeatmapLevelCollection[], int>, PlaylistViewButtonsController>("LevelCollectionTableViewUpdatedEvent");
            LevelCollectionTableViewUpdatedEvent?.Invoke(_downloadingBeatmapLevelCollections, _downloadingBeatmapCollectionIdx);
            _playlistViewButtonsController.InvokeMethod<object, PlaylistViewButtonsController>("UpdateMissingSongs");
            UpdateSecondChildControllerContentPatch.SecondChildControllerUpdatedEvent -= LevelFilteringNavigationController_UpdateSecondChildControllerContent_SecondChildControllerUpdatedEvent;
        }
    }
}

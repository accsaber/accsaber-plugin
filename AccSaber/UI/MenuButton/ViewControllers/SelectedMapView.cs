﻿using AccSaber.Downloaders;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using SiraUtil.Logging;
using TMPro;
using UnityEngine.UI;
using Zenject;
using static AccSaber.Utils.AccSaberUtils;

namespace AccSaber.UI.MenuButton.ViewControllers
{
    [ViewDefinition("AccSaber.UI.MenuButton.Views.selected-map.bsml")]
    [HotReload(RelativePathToLayout = @"..\Views\selected-map.bsml")]
    class SelectedMapView : BSMLAutomaticViewController, INotifyPropertyChanged
    {
        [Inject]
        private SiraLog _siraLog;

        [Inject]
        AccSaberDownloader _accSaberDownloader;

        [Inject]
        private AccSaberMainFlowCoordinator _accSaberMainFlowCoordinator;

        public event PropertyChangedEventHandler PropertyChanged;

        AccSaberSong _selectedSong = null;

        private bool _songCoreReady = false;

        internal static CancellationTokenSource coverCancel { get; set; } = null;

        [UIValue("song-select-ready")]
        public bool SongSelectReady
        {
            get => _selectedSong != null && _songCoreReady;
        }

        #region UIComponents
        [UIComponent("cover")]
        public readonly Image coverImage = null;

        [UIComponent("artistSongNameString")]
        TextMeshProUGUI artistSongNameString;

        [UIComponent("levelAuthorName")]
        TextMeshProUGUI levelAuthorName;

        [UIComponent("downloadButton")]
        Button downloadButton;
        //[UIComponent("playButton")]
        //Button playButton;
        #endregion

        [UIValue("diffs")]
        public List<AccSaberSongDiff> Diffs
        {
            get => _selectedSong != null ? _selectedSong.diffs : null;
        }

        //[UIAction("clicked-play")]
        //public void ClickedPlay()
        //{
        //    _siraLog.Debug("play");
        //}

        [UIAction("#post-parse")]
        void Parsed()
        {
            if (_selectedSong != null)
            {
                if (_selectedSong.cover != null)
                {
                    coverImage.sprite = _selectedSong.cover;
                }
                else
                {
                    coverImage.sprite = SongCore.Loader.defaultCoverImage;
                }

                artistSongNameString.text = _selectedSong.artistSongNameString;
                levelAuthorName.text = _selectedSong.levelAuthorName;
                SetActiveButton(!SongCore.Collections.songWithHashPresent(_selectedSong.songHash));
            }
        }

        #region internal
        internal async void SetSelectedSong(AccSaberSong song)
        {
            _selectedSong = song;
            if (artistSongNameString)
                artistSongNameString.text = _selectedSong.artistSongNameString;
            if (levelAuthorName)
                levelAuthorName.text = _selectedSong.levelAuthorName;

            coverCancel?.Cancel();
            coverCancel = new CancellationTokenSource();
            if (SongCore.Loader.AreSongsLoaded)
            {
                if (coverImage)
                {
                    if (_selectedSong == song)
                    {
                        coverImage.sprite = song.cover != null ? song.cover : SongCore.Loader.defaultCoverImage;
                    }
                }

                _songCoreReady = true;
                var songDownloaded = SongCore.Collections.songWithHashPresent(song.songHash);
                SetActiveButton(!songDownloaded);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SongSelectReady)));

                if (song.cover == null)
                {
                    if (songDownloaded)
                    {
                        song.cover = await SongCore.Loader.CustomLevels.Values.First(x => x.levelID == song.levelID).GetCoverImageAsync(coverCancel.Token);
                    }
                    else
                    {
                        song.cover = await _accSaberDownloader.GetCoverImageAsync(song.songHash, coverCancel.Token);
                    }
                    if (_selectedSong == song)
                    {
                        coverImage.sprite = song.cover;
                    }
                }

            }
            else
            {
                _songCoreReady = false;
            }
        }

        internal void SongCoreReady()
        {
            if (_songCoreReady && _selectedSong != null)
            {
                if (SongCore.Collections.songWithHashPresent(_selectedSong.songHash))
                {
                    if (downloadButton.gameObject.activeSelf)
                    {
                        SetActiveButton(false);
                    }
                }
            }
            else
            {
                _songCoreReady = true;
                if (_selectedSong != null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SongSelectReady)));
                }
            }

        }

        internal void MultiDownloadStarted()
        {
            if (downloadButton != null && downloadButton.interactable)
            {
                SetDownloadButtonDownloading();
            }
        }
        #endregion

        private void SetActiveButton(bool download)
        {
            if (download)
            {
                if (_accSaberMainFlowCoordinator.IsDownloading())
                {
                    SetDownloadButtonDownloading();
                }
            }
            downloadButton.interactable = download;
            downloadButton.SetButtonText($"{(download ? "Download" : "Downloaded")}");
            //downloadButton.gameObject.SetActive(download);
            //playButton.gameObject.SetActive(!download);
        }

        private void SetDownloadButtonDownloading()
        {
            downloadButton.interactable = false;
            downloadButton.SetButtonText("Downloading...");
        }

        #region Downloading
        [UIAction("clicked-download")]
        public void ClickedDownload()
        {
            var song = _selectedSong;
            downloadButton.SetButtonText("Downloading...");
            downloadButton.interactable = false;
            _accSaberMainFlowCoordinator.StartDownloading(false, song);
        }
        #endregion
    }
}

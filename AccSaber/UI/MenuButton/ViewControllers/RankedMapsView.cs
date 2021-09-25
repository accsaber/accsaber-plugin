using AccSaber.Utils;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using IPA.Utilities;
using SiraUtil.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static AccSaber.Utils.AccSaberUtils;

namespace AccSaber.UI.MenuButton.ViewControllers
{
    [ViewDefinition("AccSaber.UI.MenuButton.Views.ranked-maps.bsml")]
    [HotReload(RelativePathToLayout = @"..\Views\ranked-maps.bsml")]
    internal class RankedMapsView : BSMLAutomaticViewController, INotifyPropertyChanged
    {
        [Inject]
        private SiraLog _siraLog;

        // Circular injection my beloved
        [Inject]
        private AccSaberMainFlowCoordinator _accSaberMainFlowCoordinator;

        private bool _dataLoading = true;

        public event PropertyChangedEventHandler PropertyChanged;

        internal List<AccSaberSongBSML> rankedSongs = new List<AccSaberSongBSML>();

        [UIComponent("ranked-songs-list")]
        public CustomCellListTableData rankedSongsList = null;

        [UIValue("data-loading")]
        private bool DataLoading
        {
            get => _dataLoading;
            set
            {
                _dataLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DataLoading)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DataLoaded)));
            }
        }

        [UIValue("data-loaded")]
        private bool DataLoaded
        {
            get => !DataLoading;
        }

        #region SortAndFilter
        internal List<AccSaberSongBSML> filteredSongs = new List<AccSaberSongBSML>();

        [UIComponent("sort-dropdown")]
        private DropdownWithTableView _sortDropdown = null;
        [UIComponent("filter-dropdown")]
        private DropDownListSetting _filterDropdown = null;

        [UIAction("update-sort")]
        public void UpdateSort(string sort)
        {
            if (sort == null)
            {
                sort = selectedSort;
            }
            lock (filteredSongs)
            {
                filteredSongs = filteredSongs.OrderBy(sortingOptions[sort]).ToList();
                rankedSongsList.data = filteredSongs.ToList<object>();
                rankedSongsList.tableView.ReloadData();
            }
        }

        [UIAction("update-filter")]
        public void UpdateFilter(string filter)
        {
            if (filter == null)
            {
                filter = selectedFilter;
            }
            lock (filteredSongs)
            {
                filteredSongs = rankedSongs.Where(song => filteringOptions[filter](song)).ToList();
            }
            SetMissingDownloadButtonStatus();
            UpdateSort(null);
        }

        static readonly IReadOnlyDictionary<string, Func<AccSaberSongBSML, IComparable>> sortingOptions = new Dictionary<string, Func<AccSaberSongBSML, IComparable>>()
        {
            {"Title", song => song.songName},
            {"Lowest Complexity", song => song.diffs.Min(diff => diff.complexity)},
            {"Highest Complexity", song => -song.diffs.Max(diff => diff.complexity)},
            {"Category", song => song.diffs.Min(diff => diff.categoryDisplayName)}
        };

        static Dictionary<string, Func<AccSaberSongBSML, bool>> filteringOptions = new Dictionary<string, Func<AccSaberSongBSML, bool>>()
        {
            {"All", song => true },
            {"Not Downloaded", song => !song.IsDownloaded() },
            {"Overall Ranked", song => song.diffs.Any(diff => AccSaberUtils.GetCategoryByDisplayName(diff.categoryDisplayName).countsTowardsOverall) }
        };

        static readonly IReadOnlyList<object> sortOptions = sortingOptions.Select(x => x.Key).ToList<object>();
        private static List<object> filterOptions = filteringOptions.Select(x => x.Key).ToList<object>();

        internal static string selectedSort { get; private set; } = sortingOptions.First().Key;
        internal static string selectedFilter { get; private set; } = filteringOptions.First().Key;

        internal void SetFilters(List<AccSaberCategory> categories)
        {
            foreach (var category in categories)
            {
                filteringOptions.Add(category.categoryDisplayName, song => song.diffs.Any(diff => diff.categoryDisplayName == category.categoryDisplayName));
            }
            filterOptions = filteringOptions.Select(x => x.Key).ToList<object>();

            _filterDropdown.values = filterOptions;
            _filterDropdown.UpdateChoices();
        }
        #endregion

        internal void SetRankedMaps(List<AccSaberSongBSML> songs)
        {
#if DEBUG
            _siraLog.Debug("setting ranked maps");
#endif
            rankedSongs = songs;

            if (rankedSongsList != null)
            {
                UpdateFilter(null);
                UpdateSort(null);
                DataLoading = false;
            }
        }

        [UIAction("#post-parse")]
        void Parsed()
        {
            _progressBackground = progressBar.background as ImageView;
            _progressBackground.material = Utilities.ImageResources.NoGlowMat;
            _progressBackground.type = Image.Type.Filled;
            _progressBackground.fillMethod = Image.FillMethod.Horizontal;
            _progressBackground.fillOrigin = (int) Image.OriginHorizontal.Left;
            HideProgressBar();

            filterOptions = filteringOptions.Select(x => x.Key).ToList<object>();
            if (rankedSongs.Count > 0)
            {
                UpdateFilter(null);
                DataLoading = false;
            }
        }

        [UIAction("selected-cell")]
        void OnSelectCell(TableView tableView, object selectedSong)
        {
            AccSaberMainFlowCoordinator.SetSelectedSong((AccSaberSong)selectedSong);
        }

        internal void SongCoreReady()
        {
            if (rankedSongsList != null)
            {
                rankedSongsList.tableView.ReloadData();
                SetMissingDownloadButtonStatus();
            }
        }

        #region Downloading
        private HashSet<AccSaberSong> missingSongs = new HashSet<AccSaberSong>();
        private bool _isDownloading = false;

        [UIComponent("download-status")]
        TextMeshProUGUI downloadStatus;

        [UIComponent("download-missing-button")]
        Button downloadMissingButton;

        [UIComponent("progress-bar")] 
        Backgroundable progressBar = null;

        private ImageView _progressBackground;

        [UIAction("clicked-download-missing")]
        public void ClickedDownloadMissing()
        {
            _isDownloading = true;
            _accSaberMainFlowCoordinator.StartDownloading(true);
#if DEBUG
            _siraLog.Debug("clicked download missing");
#endif
            HashSet<AccSaberSong> missingSongsCopy = new HashSet<AccSaberSong>();
            lock (missingSongs)
            {
                AccSaberSong[] missingSongsArrayCopy = new AccSaberSong[missingSongs.Count];
                missingSongs.CopyTo(missingSongsArrayCopy);
                missingSongsCopy = missingSongsArrayCopy.ToHashSet<AccSaberSong>();
            }
            downloadMissingButton.interactable = false;

            DownloadSongs(missingSongsCopy);
            
        }

        private async void DownloadSongs(HashSet<AccSaberSong> missingSongsCopy)
        {
            var count = 1;
            foreach (var song in missingSongsCopy)
            {
#if DEBUG
                _siraLog.Debug($"Downloading {count} / {missingSongsCopy.Count}");
#endif
                downloadMissingButton.SetButtonText($"Downloading {count} / {missingSongsCopy.Count}");
                await DownloadSong(song);
                count++;
            }
#if DEBUG
            _siraLog.Debug("Done downloading");
#endif
            HideProgressBar();
            downloadStatus.text = "";
            _isDownloading = false;
            SongCore.Loader.Instance.RefreshSongs(false);
        }

        private void HideProgressBar()
        {
            var progressBarColor = Color.white;
            progressBarColor.a = 0;
            _progressBackground.fillAmount = 0;
            _progressBackground.color = progressBarColor;
        }

        private void SetMissingDownloadButtonStatus()
        {
            if (_isDownloading)
            {
                return;
            }
            missingSongs.Clear();
            lock (filteredSongs)
            {
                foreach (var song in filteredSongs)
                {
                    if (!song.IsDownloaded())
                    {
                        missingSongs.Add(song);
                    }
                }
            }

            if (missingSongs.Count > 0)
            {
                downloadMissingButton.interactable = true;
                downloadMissingButton.SetButtonText($"Download {missingSongs.Count} missing song{(missingSongs.Count > 1 ? "s" : "")}");
            }
            else
            {
                downloadMissingButton.interactable = false;
                downloadMissingButton.SetButtonText("All songs downloaded");
            }
        }

        internal async void DownloadSongInternal(AccSaberSong song)
        {
            _isDownloading = true;
            downloadMissingButton.interactable = false;
            downloadMissingButton.SetButtonText("Downloading...");
            await DownloadSong(song);
            HideProgressBar();
            downloadStatus.text = "";
            _isDownloading = false;
            SongCore.Loader.Instance.RefreshSongs(false);
        }

        private async Task DownloadSong(AccSaberSong song)
        {
            if (!song.IsDownloaded())
            {
                var successfulDownload = await _accSaberMainFlowCoordinator.DownloadSong(song, RefreshProgress, RefreshStatus);
            }
        }

        private void RefreshProgress(float progress)
        {
            _progressBackground.color = progress < 0.33f ? new Color(0.91f, 0.57f, 0.06f, 1) : progress < 0.67f ? Color.yellow : Color.green;
            _progressBackground.fillAmount = progress;
        }

        private void RefreshStatus(string status)
        {
            downloadStatus.text = status;
        }

        internal bool IsDownloading()
        {
            return _isDownloading;
        }
        #endregion
    }

    public class AccSaberSongBSML : AccSaberSong
    {
        public AccSaberSongBSML(string inSongName, string inSongSubName, string inSongAuthorName, string inLevelAuthorName, string inBeatSaverKey, string inSongHash, List<AccSaberSongDiff> inDiffs) :
            base(inSongName, inSongSubName, inSongAuthorName, inLevelAuthorName, inBeatSaverKey, inSongHash, inDiffs) {}

        #region Background
        [UIComponent("background-container")]
        private ImageView _background = null;

        [UIAction("refresh-visuals")]
        public void Refresh(bool selected, bool highlighted)
        {
            var alpha = selected ? 0.8f : highlighted ? 0.7f : 0.5f;
            _background.color = new Color(0, 0, 0, alpha);
        }
        #endregion
    }
}

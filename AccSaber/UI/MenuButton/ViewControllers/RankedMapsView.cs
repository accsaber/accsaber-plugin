using AccSaber.Utils;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using IPA.Utilities;
using SiraUtil.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        private bool _dataLoading = true;

        public event PropertyChangedEventHandler PropertyChanged;

        internal List<AccSaberSong> rankedSongs = new List<AccSaberSong>();

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
        internal List<AccSaberSong> filteredSongs = new List<AccSaberSong>();

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
            filteredSongs = filteredSongs.OrderBy(sortingOptions[sort]).ToList();
            rankedSongsList.data = filteredSongs.ToList<object>();
            rankedSongsList.tableView.ReloadData();
        }

        [UIAction("update-filter")]
        public void UpdateFilter(string filter)
        {
            if (filter == null)
            {
                filter = selectedFilter;
            }
            filteredSongs = rankedSongs.Where(song => filteringOptions[filter](song)).ToList();
            UpdateSort(null);
        }

        static readonly IReadOnlyDictionary<string, Func<AccSaberSong, IComparable>> sortingOptions = new Dictionary<string, Func<AccSaberSong, IComparable>>()
        {
            {"Title", song => song.songName},
            {"Lowest Complexity", song => song.diffs.Min(diff => diff.complexity)},
            {"Highest Complexity", song => -song.diffs.Max(diff => diff.complexity)},
            {"Category", song => song.diffs.Min(diff => diff.categoryDisplayName)}
        };

        static Dictionary<string, Func<AccSaberSong, bool>> filteringOptions = new Dictionary<string, Func<AccSaberSong, bool>>()
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

        internal void SetRankedMaps(List<AccSaberSong> songs)
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
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Downloaders;
using AccSaber.Interfaces;
using BeatSaberMarkupLanguage.ViewControllers;
using AccSaber.Models;
using AccSaber.Utils;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;


namespace AccSaber.UI.Leaderboard
{
    [HotReload(RelativePathToLayout = @"..\Leaderboard\AccSaberLeaderboardView.bsml")]
    [ViewDefinition("AccSaber.UI.Leaderboard.AccSaberLeaderboardView.bsml")]
    public class AccSaberLeaderboardViewController : BSMLAutomaticViewController, ILeaderboardEntriesUpdater
    {
        [Inject] private SiraLog _log;
        [Inject] private LevelCollectionNavigationController _collectionNavigation;
        [Inject] private readonly List<ILeaderboardSource> _leaderboardSource;
        [Inject] private List<AccSaberLeaderboardEntry> _leaderboardEntries;
        [Inject] private UserInfoDownloader _infoDownloader;
        [Inject] private AccSaberCategory _categories;
        [Inject] private UserIDUtils _userID;

        private GameObject _loadingControl;

        private int _selectedCellIndex;


        private List<Button> infoButtons;
        private IDifficultyBeatmap difficultyBeatmap;
        private readonly CancellationTokenSource _cancellationToken = new();

        private int pageNumber;
        public event Action<IDifficultyBeatmap, ILeaderboardSource, int> PageRequested;

        [UIComponent("leaderboard")] private readonly Transform leaderboardTransform;

        [UIComponent("leaderboard")] private readonly LeaderboardTableView leaderboard;

        #region Info Buttons

        [UIComponent("button1")] private readonly Button button1;

        [UIComponent("button2")] private readonly Button button2;

        [UIComponent("button3")] private readonly Button button3;

        [UIComponent("button4")] private readonly Button button4;

        [UIComponent("button5")] private readonly Button button5;

        [UIComponent("button6")] private readonly Button button6;

        [UIComponent("button7")] private readonly Button button7;

        [UIComponent("button8")] private readonly Button button8;

        [UIComponent("button9")] private readonly Button button9;

        [UIComponent("button10")] private readonly Button button10;

        #endregion

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            PageNumber = 0;
        }

        private int PageNumber
        {
            get => pageNumber;
            set
            {
                pageNumber = value;
                NotifyPropertyChanged(nameof(UpEnabled));
                if (leaderboard != null && _loadingControl != null && difficultyBeatmap != null)
                {
                    leaderboard.SetScores(new List<LeaderboardTableView.ScoreData>(), 0);
                    _loadingControl.SetActive(true);
                }
                PageRequested?.Invoke(difficultyBeatmap, _leaderboardSource[SelectedCellIndex], value);
            }
        }

        private int SelectedCellIndex
        {
            get => _selectedCellIndex;
            set
            {
                _selectedCellIndex = value;
                PageNumber = 0;
            }
        }

        private async Task SetScores(List<AccSaberLeaderboardEntry> leaderboardEntries)
        {
            var scores = new List<LeaderboardTableView.ScoreData>();
            var myScorePos = -1;

            if (infoButtons != null)
            {
                await IPA.Utilities.Async.UnityMainThreadTaskScheduler.Factory.StartNew(() =>
                {
                    foreach (var button in infoButtons)
                    {
                        button.gameObject.SetActive(false);
                    }
                });
            }

            if (leaderboardEntries == null || leaderboardEntries.Count == 0)
            {
                scores.Add(new LeaderboardTableView.ScoreData(0,
                    "<size=75%>Scores have yet to be refreshed. Please allow up to 15 min...</size>",
                    0, false));
                _log.Debug("Set non-refreshed leaderboard");
            }
            else
            {
                var userID = _userID.UserInfo.platformUserId;

                await IPA.Utilities.Async.UnityMainThreadTaskScheduler.Factory.StartNew(() =>
                {
                    for (var i = 0; i < (leaderboardEntries.Count > 10 ? 10 : leaderboardEntries.Count); i++)
                    {
                        scores.Add(new LeaderboardTableView.ScoreData(leaderboardEntries[i].score,
                            $"<color=#FFFFFF><size=90%>{leaderboardEntries[i].name}</size></color> - <size=70%>(<color=yellow>{leaderboardEntries[i].acc:P2}</color>)</size> - <size=65%>(<color=#00FFAE>{leaderboardEntries[i].ap:F2}ap</color>)</size>",
                            leaderboardEntries[i].rank,
                            false));

                        _log.Debug("Set scores to leaderboard.");

                        if (infoButtons != null)
                        {
                            infoButtons[i].gameObject.SetActive(true);
                            var hoverHint = infoButtons[i].GetComponent<HoverHint>();
                            hoverHint.text = $"Score Set: {leaderboardEntries[i].timeSet}";
                            _log.Debug($"Set info hover hint to {hoverHint.text}");
                        }

                        if (leaderboardEntries[i].playerId == userID)
                        {
                            myScorePos = i;
                        }
                    }
                });
            }

            if (_loadingControl != null && leaderboard != null)
            {
                await IPA.Utilities.Async.UnityMainThreadTaskScheduler.Factory.StartNew(() =>
                {
                    leaderboard.SetScores(scores, myScorePos);
                    _loadingControl.SetActive(false);
                });
            }
        }

        private void ChangeButtonScale(Button button, float scale)
        {
            var buttonTransform = button.transform;
            var localScale = buttonTransform.localScale;
            buttonTransform.localScale = localScale * scale;
            infoButtons?.Add(button);
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            var leaderboardTableCells = leaderboardTransform!.GetComponentsInChildren<LeaderboardTableCell>(true);
            foreach (var leaderboardTableCell in leaderboardTableCells)
            {
                leaderboardTableCell.transform.Find("PlayerName").GetComponent<CurvedTextMeshPro>().richText = true;
            }

            _loadingControl = leaderboardTransform.Find("LoadingControl").gameObject;

            var loadingContainer = _loadingControl.transform.Find("LoadingContainer");
            loadingContainer.gameObject.SetActive(true);
            Destroy(loadingContainer.Find("Text").gameObject);
            Destroy(_loadingControl.transform.Find("RefreshContainer").gameObject);
            Destroy(_loadingControl.transform.Find("DownloadingContainer").gameObject);

            infoButtons = new List<Button>();

            ChangeButtonScale(button1, 0.425f);
            ChangeButtonScale(button2, 0.425f);
            ChangeButtonScale(button3, 0.425f);
            ChangeButtonScale(button4, 0.425f);
            ChangeButtonScale(button5, 0.425f);
            ChangeButtonScale(button6, 0.425f);
            ChangeButtonScale(button7, 0.425f);
            ChangeButtonScale(button8, 0.425f);
            ChangeButtonScale(button9, 0.425f);
            ChangeButtonScale(button10, 0.425f);
        }

        public void DifficultyBeatmapUpdated(IDifficultyBeatmap difficultyBeatmap,
            AccSaberLeaderboardEntry levelInfoEntry)
        {
            if (levelInfoEntry != null)
            {
                this.difficultyBeatmap = difficultyBeatmap;
                if (isActiveAndEnabled)
                {
                    PageNumber = 0;
                }
            }
        }

        public void LeaderboardEntriesUpdated(List<AccSaberLeaderboardEntry> leaderboardEntries)
        {
            _leaderboardEntries = leaderboardEntries;
            NotifyPropertyChanged(nameof(DownEnabled));
            _ = SetScores(leaderboardEntries);
        }

        [UIAction("cell-selected")]
        private void OnCellSelected(SegmentedControl _, int index)
        {
            SelectedCellIndex = index;
        }

        [UIAction("up-clicked")]
        private void UpClicked()
        {
            if (UpEnabled)
            {
                PageNumber--;
            }
        }

        [UIAction("down-clicked")]
        private void DownClicked()
        {
            if (DownEnabled)
            {
                PageNumber++;
            }
        }

        [UIValue("cell-data")]
        private List<IconSegmentedControl.DataItem> CellData
        {
            get
            {
                return _leaderboardSource.Select(leaderboardSource => 
                    new IconSegmentedControl.DataItem(leaderboardSource.Icon, leaderboardSource.HoverHint)).ToList();
            }
        }

        [UIValue("up-enabled")]
        private bool UpEnabled =>
            PageNumber != 0 && _leaderboardSource[SelectedCellIndex].Scrollable;

        [UIValue("down-enabled")]
        private bool DownEnabled =>
            _leaderboardEntries is { Count: 10 } && _leaderboardSource[SelectedCellIndex].Scrollable;
    }
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Downloaders;
using BeatSaberMarkupLanguage.ViewControllers;
using AccSaber.Models;
using AccSaber.Utils;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;


namespace AccSaber.UI.Leaderboard
{
    [HotReload(RelativePathToLayout = @"..\Leaderboard\AccSaberLeaderboardView.bsml")]
    [ViewDefinition("AccSaber.UI.Leaderboard.AccSaberLeaderboardView.bsml")]
    public class AccSaberLeaderboardViewController : BSMLAutomaticViewController
    {
        [Inject]
        private SiraLog _log;
        [Inject] 
        private LevelCollectionNavigationController _collectionNavigation;
        private AccSaberDownloader _accSaberDownloader;
        private GameObject _loadingControl;
        private int _leaderboardPageNumber = 1;
        private int _selectedCellIndex;
        [Inject]
        private List<AccSaberLeaderboardEntries> _leaderboardEntries;
        [Inject]
        private AccSaberCategory _categories;
        private UserIDUtils _userID;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        
        private List<Button> infoButtons;
        
        [UIComponent("leaderboard")]
        private readonly Transform leaderboardTransform;

        [UIComponent("leaderboard")]
        private readonly LeaderboardTableView leaderboard;
        
        #region Info Buttons

        [UIComponent("button1")] 
        private readonly Button button1;

        [UIComponent("button2")]
        private readonly Button button2;

        [UIComponent("button3")]
        private readonly Button button3;

        [UIComponent("button4")]
        private readonly Button button4;

        [UIComponent("button5")]
        private readonly Button button5;

        [UIComponent("button6")]
        private readonly Button button6;

        [UIComponent("button")]
        private readonly Button button7;

        [UIComponent("button8")]
        private readonly Button button8;

        [UIComponent("button9")]
        private readonly Button button9;

        [UIComponent("button10")]
        private readonly Button button10;

        #endregion
        
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            PageNumber = 0;
        }

        private int PageNumber
        {
            get => _leaderboardPageNumber;
            set
            {
                _leaderboardPageNumber = value;
                NotifyPropertyChanged(nameof(UpEnabled));
                if (leaderboardTransform != null)
                {
                    leaderboard.SetScores(new List<LeaderboardTableView.ScoreData>(), 0);
                    _loadingControl.SetActive(true);
                }
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

        private async Task SetScores(List<AccSaberLeaderboardEntries> leaderboardEntries)
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

            var beatmap = _collectionNavigation.selectedDifficultyBeatmap;

            leaderboardEntries = await _accSaberDownloader.GetLeaderboardsAsync(
                beatmap.level.levelID.GetRankedSongHash(),
                beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName,
                beatmap.difficulty.ToString(),
                PageNumber,
                10,
                _cancellationTokenSource.Token);
            
            _log.Info("Finished request.");

            if (leaderboardEntries == null || leaderboardEntries.Count == 0)
            {
                scores.Add(new LeaderboardTableView.ScoreData(0, 
                    "<size=75%>Scores have yet to be refreshed. Please allow up to 15 minutes.</size>",
                    0, false));
            }
            else
            {
                var userID = await _userID.GetUserID();
                
                await IPA.Utilities.Async.UnityMainThreadTaskScheduler.Factory.StartNew(() =>
                {
                    for (var i = 0; i < (leaderboardEntries.Count > 10 ? 10 : leaderboardEntries.Count); i++)
                    {
                        scores.Add(new LeaderboardTableView.ScoreData(leaderboardEntries[i].score,
                            $"<color={leaderboardEntries[i].name}><size=85%>",
                            leaderboardEntries[i].rank,
                            false));
                        
                        if (infoButtons != null)
                        {
                            infoButtons[i].gameObject.SetActive(true);
                            var hoverHint = infoButtons[i].GetComponent<HoverHint>();
                            hoverHint.text = $"Score Set: {leaderboardEntries[i].timeSet}";
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
                    _loadingControl.SetActive(false);
                    leaderboard.SetScores(scores, myScorePos);
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

            // ChangeButtonScale(button1, 0.425f);
            // ChangeButtonScale(button2, 0.425f);
            // ChangeButtonScale(button3, 0.425f);
            // ChangeButtonScale(button4, 0.425f);
            // ChangeButtonScale(button5, 0.425f);
            // ChangeButtonScale(button6, 0.425f);
            // ChangeButtonScale(button7, 0.425f);
            // ChangeButtonScale(button8, 0.425f);
            // ChangeButtonScale(button9, 0.425f);
            // ChangeButtonScale(button10, 0.425f);
        }
        
        public void LeaderboardEntriesUpdated(List<AccSaberLeaderboardEntries> leaderboardEntries)
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
                List<IconSegmentedControl.DataItem> list = new List<IconSegmentedControl.DataItem>();
                return list;
            }
        }

        [UIValue("up-enabled")] 
        private bool UpEnabled => PageNumber != 0;
        
        [UIValue("down-enabled")]
        private bool DownEnabled => _leaderboardEntries is { Count: 10 };
    }
}

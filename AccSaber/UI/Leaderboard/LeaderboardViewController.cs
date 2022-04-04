using System.Collections.Generic;
using BeatSaberMarkupLanguage.ViewControllers;
using AccSaber.Models;
using AccSaber.Utils;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
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

        private GameObject _loadingControl;

        private int _leaderboardPageNumber;
        private int _selectedCellIndex;

        [Inject] private List<AccSaberLeaderboardEntries> _leaderboardEntries;
        [Inject] private AccSaberCategory _categories;
        private UserIDUtils _userID;
        

        private int PageNumber
        {
            get => _leaderboardPageNumber;
            set
            {
                _leaderboardPageNumber = value;
                NotifyPropertyChanged(nameof(UpEnabled));
                if (leaderboardTransform == null) return;
                leaderboard.SetScores(new List<LeaderboardTableView.ScoreData>(), 0);
                _loadingControl.gameObject.SetActive(true);
            }
        }

        public async void SetScore(List<AccSaberLeaderboardEntries> leaderboardEntries)
        {
            List<LeaderboardTableView.ScoreData> scores = new List<LeaderboardTableView.ScoreData>();
            int myScorePos = -1;

            if (leaderboardEntries == null || leaderboardEntries.Count == 0)
            {
                scores.Add(new LeaderboardTableView.ScoreData(0,
                    "This leaderboard has no updated scores, please be patient while it updates.",
                    0,
                    false));
            } 
            else
            {
                for (var i = 0; i < (leaderboardEntries.Count > 10 ? 10 : leaderboardEntries.Count); i++)
                {
                    scores.Add(new LeaderboardTableView.ScoreData(
                        leaderboardEntries[i].score,
                        $"<size=85%>{leaderboardEntries[i].name} - " +
                        $"<size=75%>(<color=#FFD42A>{leaderboardEntries[i].acc.ToString("F2")}%</color>)</size></size> - " +
                        $"<size=75%> (<color=#aa6eff>{leaderboardEntries[i].ap.ToString("F2")}<size=55%>ap</size></color>)</size>", 
                        leaderboardEntries[i].rank, false));

                    if (leaderboardEntries[i].playerId == await _userID.GetUserID())
                    {
                        myScorePos = i;
                    }
                }
            }

            if (leaderboardTransform != null)
            {
                _loadingControl.SetActive(false);
                leaderboard.SetScores(scores, myScorePos);
            }

            if (leaderboard == null)
            {
                _loadingControl.SetActive(true);
            }
        }
        
        [UIComponent("leaderboard")]
        private readonly Transform leaderboardTransform;

        [UIComponent("leaderboard")]
        private readonly LeaderboardTableView leaderboard;

        [UIAction("#post-parse")]
        private void PostParse()
        {
            List<LeaderboardTableView.ScoreData> placeholder = new List<LeaderboardTableView.ScoreData>();
            for (int i = 0; i < 10; i++)
            {
                placeholder.Add(new LeaderboardTableView.ScoreData(0, "", 0, false));
            }


            LeaderboardTableCell[] leaderboardTableCells =
                leaderboardTransform.GetComponentsInChildren<LeaderboardTableCell>(true);
            foreach (var cell in leaderboardTableCells)
            {
                cell.transform.Find("PlayerName").GetComponent<CurvedTextMeshPro>().richText = true;
            }
            Destroy(leaderboardTransform.Find("LoadingControl").Find("LoadingContainer").Find("Text").gameObject);
            _loadingControl = leaderboardTransform.Find("LoadingControl").gameObject;
        }

        [UIValue("up-enabled")] 
        private bool UpEnabled => PageNumber != 0;
        
        [UIValue("down-enabled")]
        private bool DownEnabled => PageNumber != null && _leaderboardEntries.Count == 10;
    }
}

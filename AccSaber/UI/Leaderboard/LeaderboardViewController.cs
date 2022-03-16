using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.ViewControllers;
using System.ComponentModel;
using AccSaber.Models;
using BeatSaberAPI.DataTransferObjects;
using BeatSaberMarkupLanguage.Attributes;
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

        private int _leaderboardPageNumber;
        private int _selectedCellIndex;

        private List<AccSaberLeaderboardEntries> _leaderboardEntries;
        
        [Inject]
        private void Construct()
        {
            
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
                }
            }
        }
        
        [UIComponent("leaderboard")]
        private readonly Transform leaderboardTransform;

        [UIComponent("leaderboard")]
        private readonly LeaderboardTableView leaderboard;

        [UIValue("up-enabled")] 
        private bool UpEnabled => PageNumber != 0;
        
        [UIValue("down-enabled")]
        private bool DownEnabled => PageNumber != null && _leaderboardEntries.Count == 10;
    }
}

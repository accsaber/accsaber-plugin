using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.ViewControllers;
using System.ComponentModel;
using System.Runtime.InteropServices;
using LeaderboardCore.Models.UI.ViewControllers;
using SiraUtil.Tools;
using Zenject;


namespace AccSaber.UI.Leaderboard
{
    public class AccSaberLeaderboardViewController : BasicLeaderboardViewController
    {
        [Inject] 
        private SiraLog _log;
        protected override bool useAroundPlayer => true;
        protected override bool useFriends => true;

        public void OnEnable()
        {
            isLoaded = false;
            didSelectLeaderboardScopeEvent -= OnSelectLeaderboardScope;
            didSelectLeaderboardScopeEvent += OnSelectLeaderboardScope;
        }

        private void OnSelectLeaderboardScope(LeaderboardScope scope)
        {
            switch (scope)
            {
                case LeaderboardScope.Global:
                    _log.Info("Set scope to Global");
                    break;
                case LeaderboardScope.AroundPlayer:
                    _log.Info("Set scope to Around Player");
                    break;
                case LeaderboardScope.Friends:
                    _log.Info("Set scope to Friends");
                    break;
            }
        }

        public void SetScoresInternal(List<LeaderboardTableView.ScoreData> scores, int scorePosition)
        {
            SetScores(scores, scorePosition);
            isLoaded = true;
        }

        protected override void OnUpClicked()
        {
            _log.Info("Up Clicked");
        }
        
        protected override void OnDownClicked()
        {
            _log.Info("Down Clicked");
        }
    }
}

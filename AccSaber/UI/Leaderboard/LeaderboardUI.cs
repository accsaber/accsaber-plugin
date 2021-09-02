using System;
using AccSaber.UI.Leaderboard.Panel;
using HMUI;
using LeaderboardCore.Managers;
using LeaderboardCore.Models;
using Zenject;

namespace AccSaber.UI.Leaderboard
{
    public class LeaderboardUI : CustomLeaderboard, IInitializable, IDisposable
    {
        private readonly ViewController accSaberPanelController;
        protected override ViewController panelViewController => accSaberPanelController;
        
        private readonly ViewController mainLeaderboardViewController;
        protected override ViewController leaderboardViewController => mainLeaderboardViewController;
        
        public LeaderboardUI(AccSaberPanelController accSaberPanelController, AccSaberLeaderboardViewController mainLeaderboardViewController)
        {
            this.accSaberPanelController = accSaberPanelController;
            this.mainLeaderboardViewController = mainLeaderboardViewController;
        }
    
        public void Initialize()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}

using AccSaber.UI.Leaderboard;
using AccSaber.UI.Leaderboard.Panel;
using HMUI;
using LeaderboardCore.Managers;
using LeaderboardCore.Models;
using SiraUtil.Tools;

namespace AccSaber.Managers
{
    class AccSaberManager : CustomLeaderboard
    {
        private CustomLeaderboardManager _customLeaderboardManager;
        
        private readonly ViewController _accSaberPanelController;
        protected override ViewController panelViewController => _accSaberPanelController;
        
        private readonly ViewController _mainLeaderboardViewController;
        protected override ViewController leaderboardViewController => _mainLeaderboardViewController;

        public AccSaberManager(SiraLog log, AccSaberPanelController accSaberPanelController, AccSaberLeaderboardViewController mainLeaderboardViewController, CustomLeaderboardManager customLeaderboardManager)
        {
            log.Info("Initializing instance...");
            _customLeaderboardManager = customLeaderboardManager;
            _mainLeaderboardViewController = mainLeaderboardViewController;
            _accSaberPanelController = accSaberPanelController;
            
            _customLeaderboardManager.Register(this);
        }
    }
}
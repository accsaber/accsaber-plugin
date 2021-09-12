using AccSaber.UI.Leaderboard;
using AccSaber.UI.Panel;
using HMUI;
using LeaderboardCore.Managers;
using LeaderboardCore.Models;
using SiraUtil.Tools;
using Zenject;

namespace AccSaber.Managers
{
    class AccSaberManager : CustomLeaderboard
    {
        private CustomLeaderboardManager _customLeaderboardManager;
        
        private readonly ViewController _accSaberPanelController;
        protected override ViewController panelViewController => _accSaberPanelController;
        
        private readonly ViewController _mainLeaderboardViewController;
        protected override ViewController leaderboardViewController => _mainLeaderboardViewController;
        private SiraLog _log;
        
        public AccSaberManager(AccSaberPanelController accSaberPanelController, AccSaberLeaderboardViewController mainLeaderboardViewController, CustomLeaderboardManager customLeaderboardManager, SiraLog log)
        {
            _customLeaderboardManager = customLeaderboardManager;
            _log = log;
            _mainLeaderboardViewController = mainLeaderboardViewController;
            _accSaberPanelController = accSaberPanelController;
            _log.Info("Registering leaderboard...");
            _customLeaderboardManager.Register(this);
        }
    }
}
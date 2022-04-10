using AccSaber.UI.Leaderboard;

namespace AccSaber.Interfaces
{
    internal interface INotifyViewActivated
    {
        public void ViewActivated(AccSaberLeaderboardViewController leaderboardViewController, bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling);
    }
}
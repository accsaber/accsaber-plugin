using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace AccSaber.UI.Leaderboard
{
    [ViewDefinition("AccSaber.UI.Leaderboard.LeaderboardUI.bsml")]
    [HotReload(RelativePathToLayout = @".\LeaderboardUI.bsml")]
    public class AccSaberLeaderboardViewController : BSMLAutomaticViewController
    {
        
    }
}
using System;
using System.Linq;
using AccSaber.Data;
using SiraUtil.Logging;

namespace AccSaber.Utils
{
    public class CategoryUtils
    {
        private readonly SiraLog _siraLog;
        private AccSaberData _accSaberData;
        private LevelCollectionNavigationController _navigation;

        public CategoryUtils(AccSaberData accSaberData, LevelCollectionNavigationController navigation, SiraLog siraLog)
        {
            _accSaberData = accSaberData;
            _navigation = navigation;
            _siraLog = siraLog;
        }

        internal string GetCategoryString()
        {
            switch (_accSaberData.RankedMaps.Single(x => 
                String.Equals(x.songHash, _navigation.selectedDifficultyBeatmap.level.levelID.GetRankedSongHash(), StringComparison.CurrentCultureIgnoreCase) 
                && String.Equals(x.difficulty, _navigation.selectedDifficultyBeatmap.difficulty.ToString(), StringComparison.CurrentCultureIgnoreCase)).categoryDisplayName)
            {
                case "True Acc":
                    return "true";
                case "Standard Acc":
                    return "standard";
                case "Tech Acc":
                    return "tech";
            }
            return null;
        }
    }
}
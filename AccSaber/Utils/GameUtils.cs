using SiraUtil.Logging;
using Zenject;

namespace AccSaber.Utils
{
    public class GameUtils
    {
        
        
        public static string DifficultyBeatmapToString(IDifficultyBeatmap difficultyBeatmap)
        {
            if (difficultyBeatmap.level is CustomPreviewBeatmapLevel)
            {
                var hash = difficultyBeatmap.level.levelID.GetRankedSongHash();
                var difficulty = difficultyBeatmap.difficulty.ToString();
                var characteristic = difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName;
                return $"{hash}/{characteristic}/{difficulty}";
            }

            return null;
        }
        
    }
}
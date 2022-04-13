namespace AccSaber.Utils
{
    public class GameUtils
    {
        public static string DifficultyBeatmapToString(IDifficultyBeatmap difficultyBeatmap)
        {
            if (difficultyBeatmap.level is CustomPreviewBeatmapLevel customLevel)
            {
                var hash = SongCore.Utilities.Hashing.GetCustomLevelHash(customLevel);
                var difficulty = difficultyBeatmap.difficulty.ToString();
                var characteristic = difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName;
                return $"{hash}/{characteristic}/{difficulty}";   
            }

            return null;
        }
    }
}
namespace AccSaber.Utils
{
    public class GameUtils
    {
        public static string DifficultyBeatmapToString(IDifficultyBeatmap difficultyBeatmap)
        {
            if (difficultyBeatmap.level is not CustomPreviewBeatmapLevel customLevel)
            {
                return null;
            }

            var hash = SongCore.Utilities.Hashing.GetCustomLevelHash(customLevel);
            var difficulty = difficultyBeatmap.difficulty.ToString();
            var characteristic = difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName;
            return $"{hash}/{characteristic}/{difficulty}";
        }
    }
}
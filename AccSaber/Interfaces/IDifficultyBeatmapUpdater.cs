using AccSaber.Downloaders;
using AccSaber.Models;
using JetBrains.Annotations;

namespace AccSaber.Interfaces
{
    public interface IDifficultyBeatmapUpdater
    {
        public void DifficultyBeatmapUpdated(IDifficultyBeatmap difficultyBeatmap, AccSaberLeaderboardEntries leaderboardEntries);
    }
}
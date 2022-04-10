using AccSaber.Models;
using JetBrains.Annotations;

namespace AccSaber.Interfaces
{
    public interface IDifficultyBeatmapUpdater
    {
        public void DifficultyBeatmapUpdated(AccSaberLeaderboardEntries leaderboardEntries);
    }
}
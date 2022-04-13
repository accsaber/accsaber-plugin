using System.Collections.Generic;
using AccSaber.Models;

namespace AccSaber.Interfaces
{
    internal interface ILeaderboardEntriesUpdater
    {
        public void LeaderboardEntriesUpdated(List<AccSaberLeaderboardEntry> leaderboardEntries);
    }
}
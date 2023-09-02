using System;
using AccSaber.Managers;
using AccSaber.Models.Base;
using JetBrains.Annotations;

namespace AccSaber.Models
{
    [UsedImplicitly]
    internal sealed class AccSaberRankedMap : Model
    {
        public string songName;
        public string songSubName;
        public string songAuthorName;
        public string levelAuthorName;
        public string difficulty;
        public string leaderboardId;
        public string beatSaverKey;
        public string songHash;
        public float complexity;
        public string categoryDisplayName;
        public DateTime dateRanked;
    }
}
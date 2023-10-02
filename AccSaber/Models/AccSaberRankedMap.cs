using System;
using System.Runtime.Serialization;
using AccSaber.Managers;
using AccSaber.Models.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;

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

        [JsonIgnore]
        public AccSaberStore.AccSaberMapCategories Category;
        
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Category = categoryDisplayName switch
            {
                "True Acc" => AccSaberStore.AccSaberMapCategories.True,
                "Standard Acc" => AccSaberStore.AccSaberMapCategories.Standard,
                "Tech Acc" => AccSaberStore.AccSaberMapCategories.Tech,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
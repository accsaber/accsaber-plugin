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
        [JsonProperty("songName")]
        public string SongName { get; set; } = null!;

        [JsonProperty("songSubName")]
        public string SongSubName { get; set; } = null!;

        [JsonProperty("songAuthorName")]
        public string SongAuthorName { get; set; } = null!;

        [JsonProperty("levelAuthorName")]
        public string LevelAuthorName { get; set; } = null!;

        [JsonProperty("difficulty")]
        public string Difficulty { get; set; } = null!;

        [JsonProperty("leaderboardId")]
        public string LeaderboardId { get; set; } = null!;

        [JsonProperty("beatSaverKey")]
        public string BeatSaverKey { get; set; } = null!;

        [JsonProperty("songHash")]
        public string SongHash { get; set; } = null!;

        [JsonProperty("complexity")]
        public float Complexity { get; set; }
        
        [JsonProperty("categoryDisplayName")]
        public string CategoryDisplayName { get; set; } = null!;

        [JsonProperty("dateRanked")]
        public DateTime DateRanked { get; set; }

        [JsonIgnore]
        public AccSaberStore.AccSaberMapCategories Category;
        
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Category = CategoryDisplayName switch
            {
                "True Acc" => AccSaberStore.AccSaberMapCategories.True,
                "Standard Acc" => AccSaberStore.AccSaberMapCategories.Standard,
                "Tech Acc" => AccSaberStore.AccSaberMapCategories.Tech,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
using System;
using Newtonsoft.Json;

namespace AccSaber.Models
{
    public class AccSaberLeaderboardEntries
    {
        [JsonProperty("rank")]
        public int rank;
        [JsonProperty("playerId")]
        public string playerId;
        [JsonProperty("playerName")]
        public string name;
        [JsonProperty("accuracy")]
        public float acc;
        [JsonProperty("score")]
        public int score;
        [JsonProperty("ap")]
        public float ap;
        [JsonProperty("accChamp")]
        public bool accChamp;
        [JsonProperty("timeSet")]
        public DateTime timeSet;
    }
}
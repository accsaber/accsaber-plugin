using Newtonsoft.Json;

namespace AccSaber.Models
{
    public class AccSaberAPISong
    {
        [JsonProperty("songName")]
        public string songName;
        [JsonProperty("songSubName")]
        public string songSubName;
        [JsonProperty("songAuthorName")]
        public string songAuthorName;
        [JsonProperty("levelAuthorName")]
        public string levelAuthorName;
        [JsonProperty("difficulty")]
        public string difficulty;
        [JsonProperty("beatSaverKey")]
        public string beatSaverKey;
        [JsonProperty("songHash")]
        public string songHash;
        [JsonProperty("complexity")]
        public float complexity;
        [JsonProperty("categoryDisplayName")]
        public string categoryDisplayName;
    }
}
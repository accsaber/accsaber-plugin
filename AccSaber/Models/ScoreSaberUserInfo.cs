using Newtonsoft.Json;

namespace AccSaber.Models
{
    public class ScoreSaberUserInfo
    {
        [JsonProperty("errorMessage")]
        public string? ErrorMessage { get; private set; }
    }
}
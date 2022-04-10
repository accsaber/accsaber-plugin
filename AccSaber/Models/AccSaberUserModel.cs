using Newtonsoft.Json;

namespace AccSaber.Models
{
    public class AccSaberUserModel
    {
        public int rank;
        public string playerId;
        public string playerName;
        public string hmd;
        public float averageAcc;
        public float ap;
        public float averageApPerMap;
        public int rankedPlays;
        public bool accChamp;
        
        [JsonIgnore]
        public bool Registered { get; internal set; }
    }
}
using AccSaber.Models.Base;
using JetBrains.Annotations;

namespace AccSaber.Models
{
	[UsedImplicitly]
	internal sealed class AccSaberUser : Model
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
	}
}
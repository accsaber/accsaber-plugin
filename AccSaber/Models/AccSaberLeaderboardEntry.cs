using System;
using AccSaber.Models.Base;
using JetBrains.Annotations;

namespace AccSaber.Models
{
	[UsedImplicitly]
	internal class AccSaberLeaderboardEntry : Model
	{
		public int rank;
		public string playerId;
		public string playerName;
		public float accuracy;
		public int score;
		public float ap;
		public bool accChamp;
		public DateTime timeSet;
	}
}
using System;
using AccSaber.Models.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AccSaber.Models
{
	[UsedImplicitly]
	internal class AccSaberUser : Model
	{
		public int rank;
		public string playerId;
		public string playerName;
		public string avatarUrl;
		public string? hmd;
		public float averageAcc;
		public float ap;
		public float averageApPerMap;
		public int rankedPlays;
		public bool accChamp;
	}
}
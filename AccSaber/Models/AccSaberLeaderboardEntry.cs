using System;
using AccSaber.Models.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AccSaber.Models
{
	[UsedImplicitly]
	internal sealed class AccSaberLeaderboardEntry : Model
	{
		[JsonProperty("rank")]
		public int Rank { get; set; }
		
		[JsonProperty("playerId")]
		public string PlayerId { get; set; } = null!;

		[JsonProperty("playerName")]
		public string PlayerName { get; set; } = null!;

		[JsonProperty("accuracy")]
		public float Accuracy { get; set; }
		
		[JsonProperty("score")]
		public int Score { get; set; }
		
		[JsonProperty("ap")]
		public float AP { get; set; }
		
		[JsonProperty("accChamp")]
		public bool AccChamp { get; set; }
		
		[JsonProperty("timeSet")]
		public DateTime TimeSet { get; set; }
	}
}
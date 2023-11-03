using System;
using AccSaber.Models.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AccSaber.Models
{
	[UsedImplicitly]
	internal class AccSaberUser : Model
	{
		[JsonProperty("rank")]
		public int Rank { get; set; }

		[JsonProperty("playerId")]
		public string PlayerId { get; set; } = null!;

		[JsonProperty("playerName")]
		public string PlayerName { get; set; } = null!;

		[JsonProperty("avatarUrl")]
		public string AvatarUrl { get; set; } = null!;

		[JsonProperty("hmd")]
		public string? Hmd { get; set; }
		
		[JsonProperty("averageAcc")]
		public float AverageAcc { get; set; }
		
		[JsonProperty("ap")]
		public float AP { get; set; }
		
		[JsonProperty("averageApPerMap")]
		public float AverageApPerMap { get; set; }
		
		[JsonProperty("rankedPlays")]
		public int RankedPlays { get; set; }
		
		[JsonProperty("accChamp")]
		public bool AccChamp { get; set; }
	}
}
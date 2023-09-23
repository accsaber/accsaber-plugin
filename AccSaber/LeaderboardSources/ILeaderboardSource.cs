using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Models;
using UnityEngine;

namespace AccSaber.LeaderboardSources
{
	internal interface ILeaderboardSource
	{
		public string HoverHint { get; }
		public Sprite Icon { get; }
		public bool Scrollable { get; }
		public Task<List<AccSaberLeaderboardEntry>?> GetScoresAsync(AccSaberRankedMap rankedMap, CancellationToken cancellationToken = default, int page = 0);
		public List<AccSaberLeaderboardEntry>? GetLatestCachedScore();
		public void ClearCache();
	}
}
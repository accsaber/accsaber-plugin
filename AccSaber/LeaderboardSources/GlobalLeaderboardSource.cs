using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Models;
using AccSaber.Utils;
using UnityEngine;

namespace AccSaber.LeaderboardSources
{
	internal sealed class GlobalLeaderboardSource : ILeaderboardSource
	{
		private readonly List<List<AccSaberLeaderboardEntry>> _cachedEntries = new();
		private Sprite? _icon;

		private readonly WebUtils _webUtils;

		public GlobalLeaderboardSource(WebUtils webUtils)
		{
			_webUtils = webUtils;
		}

		public string HoverHint => "Global";

		public Sprite Icon => _icon ??= BeatSaberMarkupLanguage.Utilities.FindSpriteInAssembly("AccSaber.Resources.GlobalIcon.png");
		
		public bool Scrollable => true;
		
		public async Task<List<AccSaberLeaderboardEntry>?> GetScoresAsync(AccSaberRankedMap rankedMap, CancellationToken cancellationToken = default, int page = 0)
		{
			if (_cachedEntries.Count >= page + 1)
			{
				return _cachedEntries[page];
			}

			var response = await _webUtils.GetAsync<List<AccSaberLeaderboardEntry>>($"https://api.accsaber.com/map-leaderboards/{rankedMap.songHash}/standard/{rankedMap.difficulty}?page={page}&pageSize=10", cancellationToken);
			if (response is null)
			{
				return null;
			}

			_cachedEntries.Add(response);
			return response;
		}

		public List<AccSaberLeaderboardEntry>? GetLatestCachedScore()
		{
			return _cachedEntries.LastOrDefault();
		}

		public void ClearCache()
		{
			_cachedEntries.Clear();
		}
	}
}
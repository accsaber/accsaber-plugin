using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Managers;
using AccSaber.Models;
using AccSaber.Utils;
using UnityEngine;

namespace AccSaber.LeaderboardSources
{
	internal sealed class AroundMeLeaderboardSource : ILeaderboardSource
	{
		private readonly List<List<AccSaberLeaderboardEntry>> _cachedEntries = new();
		private Sprite? _icon;
		
		private readonly WebUtils _webUtils;
		private readonly AccSaberStore _accSaberStore;

		public AroundMeLeaderboardSource(WebUtils webUtils, AccSaberStore accSaberStore)
		{
			_webUtils = webUtils;
			_accSaberStore = accSaberStore;
		}
		
		public string HoverHint => "Around Me";

		public Sprite Icon => _icon ??= BeatSaberMarkupLanguage.Utilities.FindSpriteInAssembly("AccSaber.Resources.PlayerIcon.png");
		
		public bool Scrollable => false;
		public async Task<List<AccSaberLeaderboardEntry>?> GetScoresAsync(AccSaberRankedMap rankedMap, CancellationToken cancellationToken = default, int page = 0)
		{
			if (_cachedEntries.Count >= page + 1)
			{
				return _cachedEntries[page];
			}

			var userInfo = await _accSaberStore.GetPlatformUserInfo();
			if (userInfo is null)
			{
				return null;
			}
			
			var response = await _webUtils.GetAsync<List<AccSaberLeaderboardEntry>>($"https://api.accsaber.com/map-leaderboards/{rankedMap.songHash}/standard/{rankedMap.difficulty}/around/{userInfo.platformUserId}", cancellationToken);
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
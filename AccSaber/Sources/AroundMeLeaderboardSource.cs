using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Interfaces;
using AccSaber.Models;
using AccSaber.Utils;
using SiraUtil.Web;
using UnityEngine;

namespace AccSaber.Sources
{
    public class AroundMeLeaderboardSource : ILeaderboardSource
    {
        public string HoverHint => "Around Me";
        public Sprite _icon;
        
        private readonly IHttpService _httpService;
        private readonly UserIDUtils _userIDUtils;
        private List<AccSaberLeaderboardEntry> leaderboardCache = new();
        
        public Sprite Icon
        {
            get
            {
                if (_icon == null)
                {
                    _icon = BeatSaberMarkupLanguage.Utilities.FindSpriteInAssembly("AccSaber.Resources.PlayerIcon.png");
                }
                return _icon;
            }
        }
        
        public async Task<List<AccSaberLeaderboardEntry>> GetScoresAsync(IDifficultyBeatmap difficultyBeatmap,
            int page = 0, CancellationToken cancellationToken = default)
        {
            var userID = _userIDUtils.UserInfo.platformUserId;
            if (leaderboardCache == null)
            {
                var beatmapString = GameUtils.DifficultyBeatmapToString(difficultyBeatmap);
                if (beatmapString == null)
                {
                    return null;
                }

                if (userID == null)
                {
                    return null;
                }
                var id = userID;

                try
                {
                    var url = Constants.API_URL + Constants.LEADERBOARDS_ENDPOINT + beatmapString +
                              Constants.PAGINATION_PAGE + page + Constants.PAGINATION_PAGESIZE + 10;
                    var webResponse = await _httpService.GetAsync(url, cancellationToken: cancellationToken).ConfigureAwait(false);
                    leaderboardCache = await ResponseParser.ParseWebResponse<List<AccSaberLeaderboardEntry>>(webResponse);
                }
                catch (TaskCanceledException) { }
            }
            return leaderboardCache;
        }

        public bool Scrollable => false;
        
        public void ClearCache() => leaderboardCache.Clear();
    }
}
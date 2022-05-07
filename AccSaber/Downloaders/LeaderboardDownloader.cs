using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Models;
using AccSaber.Utils;
using SiraUtil.Logging;
using SiraUtil.Web;

namespace AccSaber.Downloaders
{
    public class LeaderboardDownloader
    {
        private readonly IHttpService _httpService;
        private SiraLog _siraLog;
        private readonly Dictionary<IDifficultyBeatmap, List<AccSaberLeaderboardEntry>> leaderboardCache = new();

        public LeaderboardDownloader(IHttpService httpService, SiraLog siraLog)
        {
            _httpService = httpService;
            _siraLog = siraLog;
        }
        
        public async Task<List<AccSaberLeaderboardEntry>> GetLevelInfoAsync(IDifficultyBeatmap difficultyBeatmap, CancellationToken? cancellationToken = null)
        {
            if (leaderboardCache.TryGetValue(difficultyBeatmap, out var cachedValue))
            {
                _siraLog.Debug($"returning {cachedValue}");
                return cachedValue;
            }
            
            var beatmapString = GameUtils.DifficultyBeatmapToString(difficultyBeatmap);
            if (beatmapString == null)
            {
                _siraLog.Warn("Variable \"beatmapString\" is null!");
                return null;
            }

            try
            {
                var url = Constants.API_URL + Constants.LEADERBOARDS_ENDPOINT + beatmapString + Constants.PAGINATION_PAGE + 0 + Constants.PAGINATION_PAGESIZE + 10;
                _siraLog.Debug(url);
                var webResponse = 
                    await _httpService.GetAsync(url, 
                        cancellationToken: cancellationToken ?? CancellationToken.None);
                _siraLog.Debug($"Received response with code {webResponse.Code}!");
                var levelInfo = await ResponseParser.ParseWebResponse<List<AccSaberLeaderboardEntry>>(webResponse);
                _siraLog.Debug(levelInfo);

                leaderboardCache[difficultyBeatmap] = levelInfo;
                return levelInfo;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }
    }
}
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
        private const string API_URL = "https://api.accsaber.com/";
        private const string CDN_URL = "https://cdn.accsaber.com/";
        private const string COVERS_ENDPOINT = "covers/";
        private const string RANKED_ENDPOINT = "ranked-maps";
        private const string LEADERBOARDS_ENDPOINT = "map-leaderboards/";
        private const string PLAYERS_ENDPOINT = "players/";
        private const string PAGINATION_PAGE = "?page=";
        private const string PAGINATION_PAGESIZE = "&pageSize=";
        
        private readonly IHttpService _httpService;
        private readonly SiraLog _log;
        
        public LeaderboardDownloader(SiraLog siraLog, IHttpService httpService, SiraLog log)
        {
            _httpService = httpService;
            _log = log;
        }

        public async Task<AccSaberLeaderboardEntries> GetLeaderboardAsync(IDifficultyBeatmap difficultyBeatmap, CancellationToken cancellationToken = default)
        {
            var beatmapString = Utils.GameUtils.DifficultyBeatmapToString(difficultyBeatmap);
            if (beatmapString == null)
            {
                return null;
            }

            try
            {
                var response = await _httpService
                    .GetAsync(API_URL + LEADERBOARDS_ENDPOINT + beatmapString + PAGINATION_PAGE + 0 + PAGINATION_PAGESIZE + 10, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                _log.Debug(response.Code);
                var leaderboardInfo = await Utils.ResponseParser.ParseWebResponse<AccSaberLeaderboardEntries>(response);

                return leaderboardInfo;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }
    }
}
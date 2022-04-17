using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Models;
using AccSaber.Utils;
using SiraUtil.Logging;
using SiraUtil.Web;

namespace AccSaber.Downloaders
{
    public class LeaderboardDownloader : Downloader
    {
        private const string API_URL = "https://api.accsaber.com/";
        private const string CDN_URL = "https://cdn.accsaber.com/";
        private const string COVERS_ENDPOINT = "covers/";
        private const string RANKED_ENDPOINT = "ranked-maps";
        private const string LEADERBOARDS_ENDPOINT = "map-leaderboards/";
        private const string PLAYERS_ENDPOINT = "players/";
        private const string PAGINATION_PAGE = "?page=";
        private const string PAGINATION_PAGESIZE = "&pageSize=";
        
        private readonly SiraLog _log;

        public LeaderboardDownloader(SiraLog siraLog, SiraLog log) : base(siraLog)
        {
            _log = log;
        }

        public async Task<List<AccSaberLeaderboardEntry>> GetLeaderboardAsync(IDifficultyBeatmap difficultyBeatmap, int page = 0, CancellationToken cancellationToken = default)
        {
            var beatmapString = GameUtils.DifficultyBeatmapToString(difficultyBeatmap);
            if (beatmapString == null)
            {
                return null;
            }

            var url = API_URL + LEADERBOARDS_ENDPOINT + beatmapString + PAGINATION_PAGE + page + PAGINATION_PAGESIZE + 10;
            return await MakeJsonRequestAsync<List<AccSaberLeaderboardEntry>>(url, cancellationToken);
        }
    }
}
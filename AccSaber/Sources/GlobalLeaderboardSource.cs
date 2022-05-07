using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Downloaders;
using AccSaber.Interfaces;
using AccSaber.Models;
using AccSaber.Utils;
using SiraUtil.Logging;
using SiraUtil.Web;
using UnityEngine;
using Zenject;

namespace AccSaber.Sources
{
    public class GlobalLeaderboardSource : ILeaderboardSource
    {
        private readonly IHttpService _httpService;
        private readonly List<List<AccSaberLeaderboardEntry>> leaderboardCache = new();
        [Inject] private SiraLog _log;
        
        public string HoverHint => "Global";
        private Sprite _icon;
        public Sprite Icon
        {
            get
            {
                if (_icon == null)
                {
                    _icon = BeatSaberMarkupLanguage.Utilities.FindSpriteInAssembly("AccSaber.Resources.GlobalIcon.png");
                }
                return _icon;
            }
        }

        public async Task<List<AccSaberLeaderboardEntry>> GetScoresAsync(IDifficultyBeatmap difficultyBeatmap,
            int page = 0, CancellationToken cancellationToken = default)
        {
            if (leaderboardCache.Count < page + 1)
            {
                _log.Debug("knob");
                var beatmapString = GameUtils.DifficultyBeatmapToString(difficultyBeatmap);
                _log.Debug("knob2");
                if (beatmapString == null)
                {
                    _log.Debug("beatmap is null");
                    return null;
                }
                
                try
                {
                    var response = await _httpService.GetAsync(Constants.API_URL + Constants.LEADERBOARDS_ENDPOINT + beatmapString +
                                                               Constants.PAGINATION_PAGE + page + Constants.PAGINATION_PAGESIZE + 10, cancellationToken: cancellationToken);
                    _log.Debug("sent request, going to parse.");
                    var scores = await ResponseParser.ParseWebResponse<List<AccSaberLeaderboardEntry>>(response);
                    if (scores != null)
                    {
                        _log.Debug($"Adding scores from {scores} with count {scores.Count}");
                        leaderboardCache.Add(scores);
                    }
                }
                catch (TaskCanceledException)
                { }
            }
            return page < leaderboardCache.Count ? leaderboardCache[page] : null;
        }

        public bool Scrollable => true;
        public void ClearCache() => leaderboardCache.Clear();
    }
}
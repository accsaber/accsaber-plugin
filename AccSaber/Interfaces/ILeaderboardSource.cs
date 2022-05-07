using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Models;
using UnityEngine;

namespace AccSaber.Interfaces
{
    public interface ILeaderboardSource
    {
        public string HoverHint { get; }
        public Sprite Icon { get; }

        public Task<List<AccSaberLeaderboardEntry>> GetScoresAsync(IDifficultyBeatmap difficultyBeatmap,
            int page = 0, CancellationToken cancellationToken = default);

        public bool Scrollable { get; }

        public void ClearCache();
    }
}
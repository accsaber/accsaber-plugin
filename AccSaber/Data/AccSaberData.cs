using AccSaber.Downloaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Models;
using Zenject;

namespace AccSaber.Data
{
    // TODO: Switch over downloader to use this
    class AccSaberData : IInitializable, IDisposable
    {
        private AccSaberDownloader _accSaberDownloader;

        private List<AccSaberAPISong> _rankedSongs = new List<AccSaberAPISong>();
        private List<AccSaberLeaderboardEntries> _leaderboardData = new List<AccSaberLeaderboardEntries>();

        private string characteristic;
        private string difficulty;

        private Dictionary<int, List<AccSaberLeaderboardEntries>> _leaderboardHashMap = new Dictionary<int, List<AccSaberLeaderboardEntries>>();
        private Dictionary<string, List<AccSaberAPISong>> _songHashMap = new Dictionary<string, List<AccSaberAPISong>>();
        private bool _leaderboardDataInitialized = false;
        private bool _mapDataInitialized = false;

        private CancellationTokenSource _cancellationTokenSource;

        public bool IsDataInitialized
        {
            get => _mapDataInitialized || _leaderboardDataInitialized;
        }

        public List<AccSaberAPISong> RankedMaps
        {
            get => _rankedSongs;
        }

        public AccSaberData(AccSaberDownloader accSaberDownloader)
        {
            _accSaberDownloader = accSaberDownloader;
        }

        public void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            FetchRankedMaps();
            // FetchLeaderboardData();
        }

        public List<AccSaberAPISong> GetMapsFromHash(string hash)
        {
            hash = hash.ToLower();
            if (_songHashMap.ContainsKey(hash))
            {
                return _songHashMap[hash];
            }
            return new List<AccSaberAPISong>();
        }

        private async void FetchRankedMaps()
        {
            
            _rankedSongs = await _accSaberDownloader.GetRankedMapsAsync(_cancellationTokenSource.Token);
            foreach (var rankedSong in _rankedSongs)
            {
                var hash = rankedSong.songHash.ToLower();
                if (_songHashMap.ContainsKey(hash))
                {
                    _songHashMap[hash].Add(rankedSong);
                }
                else
                {
                    _songHashMap.Add(hash, new List<AccSaberAPISong>() { rankedSong });
                }
            }

            _mapDataInitialized = true;
        }

        // private async void FetchLeaderboardData()
        // {
        //     _leaderboardData = await _accSaberDownloader.GetLeaderboardsAsync(_cancellationTokenSource.Token, characteristic, difficulty);
        //     foreach (var score in _leaderboardData)
        //     {
        //         var data = score.score;
        //         if (_leaderboardHashMap.ContainsKey(data))
        //         {
        //             _leaderboardHashMap[data].Add(score);
        //         }
        //         else
        //         {
        //             _leaderboardHashMap.Add(data, new List<AccSaberLeaderboardEntries>() { score });
        //         }
        //
        //         _leaderboardDataInitialized = true;
        //     }
        // }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
        }
    }
}

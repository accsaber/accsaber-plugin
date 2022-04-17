using AccSaber.Downloaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AccSaber.Models;
using JetBrains.Annotations;
using SiraUtil.Logging;
using Zenject;

namespace AccSaber.Data
{
    // TODO: Switch over downloader to use this
    public class AccSaberData : IInitializable, IDisposable
    {
        [Inject] private SiraLog _log;
        
        private AccSaberDownloader _accSaberDownloader;

        private List<AccSaberAPISong> _rankedSongs = new List<AccSaberAPISong>();

        private Dictionary<string, List<AccSaberAPISong>> _songHashMap = new Dictionary<string, List<AccSaberAPISong>>();
        private bool _mapDataInitialized;

        private CancellationTokenSource _cancellationTokenSource;
        
        

        public bool IsMapDataInitialized
        {
            get => _mapDataInitialized;
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
                    _songHashMap.Add(hash, new List<AccSaberAPISong> { rankedSong });
                }
            }
            _log.Info("Finished caching ranked maps");
            _mapDataInitialized = true;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}

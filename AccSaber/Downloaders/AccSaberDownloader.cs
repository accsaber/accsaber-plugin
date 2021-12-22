using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Utils;
using static AccSaber.Utils.AccSaberUtils;
using SiraUtil.Logging;
using UnityEngine;

namespace AccSaber.Downloaders
{
    public class AccSaberDownloader : Downloader
    {
        private const string API_URL = "https://api.accsaber.com/";
        private const string CDN_URL = "https://cdn.accsaber.com/";
        private const string COVERS_ENDPOINT = "covers/";
        private const string RANKED_ENDPOINT = "ranked-maps";

        private const string CATEGORY_ENDPOINT = "categories";

        private readonly SiraLog _siraLog;
        private static Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

        public AccSaberDownloader(SiraLog siraLog) : base(siraLog)
        {
            _siraLog = siraLog;
        }

        public async Task<List<AccSaberAPISong>> GetRankedMapsAsync(CancellationToken cancellationToken)
        {
            string url = API_URL + RANKED_ENDPOINT;
            return await MakeJsonRequestAsync<List<AccSaberUtils.AccSaberAPISong>>(url, cancellationToken);
        }

        public async Task<List<AccSaberCategory>> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            string url = API_URL + CATEGORY_ENDPOINT;
            return await MakeJsonRequestAsync<List<AccSaberCategory>>(url, cancellationToken);
        }

        public async Task<Sprite> GetCoverImageAsync(string hash, CancellationToken cancellationToken)
        {
            hash = hash.ToUpper();
            if (_spriteCache.ContainsKey(hash))
            {
                return _spriteCache[hash];
            }
            string url = CDN_URL + COVERS_ENDPOINT + hash + ".png";

            var sprite = await MakeImageRequestAsync(url, cancellationToken);
            if (sprite != null)
            {
                _spriteCache.Add(hash, sprite);
            }

            return sprite;
        }
    }
}

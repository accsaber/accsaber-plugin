using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Models;
using AccSaber.Utils;
using IPA.Utilities;
using Newtonsoft.Json.Linq;
using static AccSaber.Utils.AccSaberUtils;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace AccSaber.Downloaders
{
    public class AccSaberDownloader : Downloader
    {
        

        private readonly SiraLog _siraLog;
        private static Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

        public AccSaberDownloader(SiraLog siraLog) : base(siraLog)
        {
            _siraLog = siraLog;
        }

        public async Task<List<AccSaberAPISong>> GetRankedMapsAsync(CancellationToken cancellationToken)
        {
            string url = Constants.API_URL + Constants.RANKED_ENDPOINT;
            return await MakeJsonRequestAsync<List<AccSaberAPISong>>(url, cancellationToken);
        }

        public async Task<List<AccSaberCategory>> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            string url = Constants.API_URL + Constants.CATEGORY_ENDPOINT;
            return await MakeJsonRequestAsync<List<AccSaberCategory>>(url, cancellationToken);
        }

        public async Task<Sprite> GetCoverImageAsync(string hash, CancellationToken cancellationToken)
        {
            hash = hash.ToUpper();
            if (_spriteCache.ContainsKey(hash))
            {
                return _spriteCache[hash];
            }
            string url = Constants.CDN_URL + Constants.COVERS_ENDPOINT + hash + ".png";

            var sprite = await MakeImageRequestAsync(url, cancellationToken);
            if (sprite != null)
            {
                _spriteCache.Add(hash, sprite);
            }

            return sprite;
        }
    }
}

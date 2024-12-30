using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SiraUtil.Logging;
using SiraUtil.Web;

namespace AccSaber.Utils
{
    internal sealed class WebUtils
    {
        private readonly SiraLog _log;
        private readonly IHttpService _httpService;

        public WebUtils(SiraLog log, IHttpService httpService)
        {
            _log = log;
            _httpService = httpService;
        }
        
        internal async Task<IHttpResponse?> GetAsync(string url, CancellationToken cancellationToken = default)
        {
            _log.Debug($"Sending GET request to {url}");
            
            try
            {
                return await _httpService.GetAsync(url, cancellationToken: cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        internal async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await GetAsync(url, cancellationToken: cancellationToken);
                
                if (response is null)
                {
                    return default;
                }

                var parsed = await ParseWebResponse<T>(response);
                return parsed;
            }
            catch (TaskCanceledException)
            {
                return default;
            }
        }

        private async Task<T?> ParseWebResponse<T>(IHttpResponse webResponse)
        {
            if (!webResponse.Successful)
            {
                _log.Error($"Unsuccessful web response for parsing {typeof(T)}. Status code: {webResponse.Code}");
                return default;
            }

            try
            {
                using var streamReader = new StreamReader(await webResponse.ReadAsStreamAsync());
                using var jsonTextReader = new JsonTextReader(streamReader);
                var jsonSerializer = new JsonSerializer();
                return jsonSerializer.Deserialize<T>(jsonTextReader);
            }
            catch (Exception e)
            {
                _log.Critical(e);
                return default;
            }
        }
    }
}
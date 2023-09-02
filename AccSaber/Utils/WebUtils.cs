using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SiraUtil.Logging;
using SiraUtil.Web;

namespace AccSaber.Utils
{
    internal class WebUtils
    {
        private readonly SiraLog _log;
        private readonly IHttpService _httpService;

        public WebUtils(SiraLog log, IHttpService httpService)
        {
            _log = log;
            _httpService = httpService;
        }

        internal async Task<T?> GetAsync<T>(string path)
        {
            var response = await _httpService.GetAsync(path);

            var parsed = await ParseWebResponse<T>(response);
            return parsed;
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
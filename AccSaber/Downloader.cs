using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AccSaber.Utils;
using IPA.Utilities;
using Newtonsoft.Json;
using SiraUtil.Web;

namespace AccSaber
{
    internal sealed class Downloader
    {
        private readonly IHttpService _httpService;

        public Downloader(IHttpService httpService)
        {
            _httpService = httpService;
        }

        internal async Task<T?> Get<T>(string path)
        {
            var response = await _httpService.GetAsync(path);

            var parsed = await WebResponse.Parse<T>(response);
            return parsed;
        }
    }
}
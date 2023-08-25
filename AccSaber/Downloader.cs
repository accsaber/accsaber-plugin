using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IPA.Utilities;
using Newtonsoft.Json;
using SiraUtil.Web;

namespace AccSaber
{
    public class Downloader
    {
        private IHttpService _http;
        internal async Task<T> Get<T>(string path)
        {
            var response = await _http.GetAsync(path);

            var parsed = await WebResponse.Parse<T>(response);
            return parsed;
        }
    }
}
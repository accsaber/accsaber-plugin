using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SiraUtil.Web;

namespace AccSaber.Utils
{
    internal static class WebResponse
    {
        public static async Task<T?> Parse<T>(IHttpResponse webResponse)
        {
            if (webResponse.Successful && (await webResponse.ReadAsByteArrayAsync()).Length > 3)
            {
                using var streamReader = new StreamReader(await webResponse.ReadAsStreamAsync());
                using var jsonTextReader = new JsonTextReader(streamReader);
                var jsonSerializer = new JsonSerializer();
                return jsonSerializer.Deserialize<T>(jsonTextReader);
            }

            if (!webResponse.Successful)
            {
                // Plugin.Log.Error($"Unsuccessful web response for parsing {typeof(T)}. Status code: {webResponse.Code}");
            }
			    
            return default;
        }
    }
}
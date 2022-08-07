using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SiraUtil.Web;

namespace AccSaber.Utils
{
    public class ResponseParser
    {
        public static async Task<T> ParseWebResponse<T>(IHttpResponse response)
        {
            if (response.Successful && (await response.ReadAsByteArrayAsync()).Length > 3)
            {
                using var streamReader = new StreamReader(await response.ReadAsStreamAsync());
                using var jsonTextReader = new JsonTextReader(streamReader);
                var jsonSerializer = new JsonSerializer();
                return jsonSerializer.Deserialize<T>(jsonTextReader);
            }
            return default;
        }
    }
}
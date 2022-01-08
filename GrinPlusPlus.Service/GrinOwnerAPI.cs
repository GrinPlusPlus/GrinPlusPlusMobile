using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    class GrinOwnerAPI : API
    {
        public static async Task<T> Request<T>(string endpoint, Dictionary<string, string> headers = null)
        {
            string response = await GrinOwnerAPI.MakeRequestAsync(endpoint, headers);

            return JsonConvert.DeserializeObject<T>(response);
        }

        private static async Task<string> MakeRequestAsync(string endpoint, Dictionary<string, string> headers = null)
        {
            string url = "http://127.0.0.1:3420/v1/wallet/owner/" + endpoint;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

            }

            return await (await httpClient.SendAsync(request)).Content.ReadAsStringAsync();
        }
    }
}

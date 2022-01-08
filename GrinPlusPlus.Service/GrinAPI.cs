using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    class GrinAPI : API
    {
        public static async Task<T> Request<T>(string endpoint)
        {
            string response = await GrinAPI.MakeRequestAsync(endpoint);

            return JsonConvert.DeserializeObject<T>(response);
        }

        public static async Task Request(string endpoint)
        {
            await MakeRequestAsync(endpoint);
        }

        private static async Task<string> MakeRequestAsync(string endpoint)
        {
            string url = "http://127.0.0.1:3413/v1/" + endpoint;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            return await (await httpClient.SendAsync(request)).Content.ReadAsStringAsync();
        }
    }
}

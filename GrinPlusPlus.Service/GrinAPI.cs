using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    class GrinAPI
    {
        static readonly HttpClient httpClient = new HttpClient() { Timeout = TimeSpan.FromMinutes(2) };

        public static async Task<T> Request<T>(string endpoint)
        {
            string response = await GrinAPI.MakeRequestAsync(endpoint).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(response);
        }

        public static async Task Request(string endpoint)
        {
            await MakeRequestAsync(endpoint).ConfigureAwait(false);
        }

        private static async Task<string> MakeRequestAsync(string endpoint)
        {
            httpClient.CancelPendingRequests();
            httpClient.DefaultRequestHeaders.Clear();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string url = "http://127.0.0.1:3413/v1/" + endpoint;
            
            return await (await httpClient.GetAsync(url).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}

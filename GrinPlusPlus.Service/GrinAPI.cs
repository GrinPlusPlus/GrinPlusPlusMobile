using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    class GrinAPI
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
            string response = string.Empty;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMinutes(2);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string url = "http://127.0.0.1:3413/v1/" + endpoint;
                response = await (await httpClient.GetAsync(url)).Content.ReadAsStringAsync();
            }

            return response;
        }
    }
}

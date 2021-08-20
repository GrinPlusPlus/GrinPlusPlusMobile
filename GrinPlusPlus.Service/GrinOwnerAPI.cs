using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    class GrinOwnerAPI
    {
        static readonly HttpClient httpClient = new HttpClient() { Timeout = TimeSpan.FromMinutes(5) };

        public static async Task<T> Request<T>(string endpoint, Dictionary<string, string> headers = null)
        {
            string response = await GrinOwnerAPI.MakeRequestAsync(endpoint, headers);

            return JsonConvert.DeserializeObject<T>(response);
        }

        private static async Task<string> MakeRequestAsync(string endpoint, Dictionary<string, string> headers = null)
        {
            httpClient.CancelPendingRequests();
            httpClient.DefaultRequestHeaders.Clear();

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

            }

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string url = "http://localhost:3420/v1/wallet/owner/" + endpoint;
            return await (await httpClient.GetAsync(url).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}

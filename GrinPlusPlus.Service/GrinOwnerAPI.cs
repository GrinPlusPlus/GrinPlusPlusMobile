using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    class GrinOwnerAPI
    {
        public static async Task<T> Request<T>(string endpoint, Dictionary<string, string> headers = null)
        {
            string response = await GrinOwnerAPI.MakeRequestAsync(endpoint, headers);

            return JsonConvert.DeserializeObject<T>(response);
        }

        private static async Task<string> MakeRequestAsync(string endpoint, Dictionary<string, string> headers = null)
        {
            string response = string.Empty;

            using (HttpClient httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    
                }
                
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string url = "http://127.0.0.1:3420/v1/wallet/owner/" + endpoint;
                response = await (await httpClient.GetAsync(url)).Content.ReadAsStringAsync();
            }

            return response;
        }
    }
}

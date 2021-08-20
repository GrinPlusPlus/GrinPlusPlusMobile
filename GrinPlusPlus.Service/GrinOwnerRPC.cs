using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    class GrinOwnerRPC
    {
        static readonly HttpClient httpClient = new HttpClient() { Timeout = TimeSpan.FromMinutes(5)};

        static string BuildPayload(string method, Dictionary<string, object> keyValuePairs)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "jsonrpc", "2.0" },
                { "method", method },
                { "id", Guid.NewGuid().ToString() },
                { "params", keyValuePairs }
            };

            return JsonConvert.SerializeObject(payload, Formatting.None);
        }

        public static async Task<T> Request<T>(string method, Dictionary<string, object> keyValuePairs,
            Dictionary<string, string> headers = null)
        {
            string response = await GrinOwnerRPC.MakeRequestAsync(BuildPayload(method, keyValuePairs), headers);

            try
            {
                var error = JsonConvert.DeserializeObject<Models.RPC.ErrorResponse>(response, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                });

                throw new Exception(error.Error.Message);
            }
            catch (JsonSerializationException)
            {
                return await Task<T>.Factory.StartNew(() =>
                {
                    var deserialized = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(deserialized["result"]));
                });
            }
        }

        public static async Task Request(string method, Dictionary<string, object> keyValuePairs, Dictionary<string, string> headers = null)
        {
            string response = await GrinOwnerRPC.MakeRequestAsync(BuildPayload(method, keyValuePairs), headers);

            var deserializeObject = JsonConvert.DeserializeObject<Models.RPC.ErrorResponse>(response);

            if (deserializeObject.Error != null)
            {
                throw new Exception(deserializeObject.Error.Message);
            }
        }

        private static async Task<string> MakeRequestAsync(string payload, Dictionary<string, string> headers = null)
        {
            httpClient.CancelPendingRequests();
            httpClient.DefaultRequestHeaders.Clear();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var result = await httpClient.PostAsync("http://localhost:3421/v2", new StringContent(payload)).ConfigureAwait(false);

            return await result.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}

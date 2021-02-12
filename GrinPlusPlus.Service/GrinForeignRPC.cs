using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    class GrinForeignRPC
    {
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

        static string BuildPayload(string method)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "jsonrpc", "2.0" },
                { "method", method },
                { "id", Guid.NewGuid().ToString() },
            };

            return JsonConvert.SerializeObject(payload, Formatting.None);
        }

        public static async Task<T> Request<T>(string method, Dictionary<string, object> keyValuePairs,
            Dictionary<string, string> headers = null)
        {
            string response = await GrinForeignRPC.MakeRequestAsync(BuildPayload(method, keyValuePairs), headers);

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

        public static async Task<T> Request<T>(string method, Dictionary<string, string> headers = null)
        {
            string response = await GrinForeignRPC.MakeRequestAsync(BuildPayload(method), headers);

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
            string response = await GrinForeignRPC.MakeRequestAsync(BuildPayload(method, keyValuePairs), headers);

            var deserializeObject = JsonConvert.DeserializeObject<Models.RPC.ErrorResponse>(response);

            if (deserializeObject.Error != null)
            {
                throw new Exception(deserializeObject.Error.Message);
            }
        }

        private static async Task<string> MakeRequestAsync(string payload, Dictionary<string, string> headers = null)
        {
            string response = string.Empty;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMinutes(10);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = await httpClient.PostAsync("http://127.0.0.1:3413/v2/foreign", new StringContent(payload));
                response = await result.Content.ReadAsStringAsync();
            }

            return response;
        }
    }
}

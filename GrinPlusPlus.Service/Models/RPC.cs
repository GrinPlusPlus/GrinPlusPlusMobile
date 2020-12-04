using Newtonsoft.Json;

namespace GrinPlusPlus.Service.Models
{
    public class RPC
    {
        public class Error
        {
            [JsonProperty("code")]
            public int Code { get; set; }

            [JsonProperty("data")]
            public string Data { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }
        public class ErrorResponse
        {
            [JsonProperty("error")]
            public Error Error { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("jsonrpc")]
            public string JsonRPC { get; set; }
        }
    }
}

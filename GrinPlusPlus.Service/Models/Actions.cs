using Newtonsoft.Json;
using static GrinPlusPlus.Service.Models.Basics;

namespace GrinPlusPlus.Service.Models
{
    public class Actions
    {
        public class Wallet
        {
            public class Create
            {
                [JsonProperty("session_token")]
                public string Token { get; set; }

                [JsonProperty("wallet_seed")]
                public string Seed { get; set; }

                [JsonProperty("listener_port")]
                public int Listener { get; set; }

                [JsonProperty("tor_address")]
                public string TorAdddress { get; set; }

                [JsonProperty("slatepack_address")]
                public string SlatepackAdddress { get; set; }
            }

            public class Open
            {
                [JsonProperty("session_token")]
                public string Token { get; set; }

                [JsonProperty("listener_port")]
                public int Listener { get; set; }

                [JsonProperty("tor_address")]
                public string TorAdddress { get; set; }

                [JsonProperty("slatepack_address")]
                public string SlatepackAdddress { get; set; }
            }

            public class Delete
            {
                [JsonProperty("error")]
                public string Error { get; set; }

                [JsonProperty("status")]
                public string Status { get; set; }
            }
        }


        public class Coins
        {
            public class Send
            {
                [JsonProperty("slatepack")]
                public string Slatepack { get; set; }

                [JsonProperty("status")]
                public string Status { get; set; }

                [JsonProperty("error")]
                public string Error { get; set; }
            }

            public class Receive
            {
                [JsonProperty("slatepack")]
                public string Slatepack { get; set; }

                [JsonProperty("status")]
                public string Status { get; set; }

                [JsonProperty("error")]
                public string Error { get; set; }
            }
        }

        public class Transaction
        {
            public class Finalize
            {
                [JsonProperty("error")]
                public string Error { get; set; }

                [JsonProperty("status")]
                public string Status { get; set; }
            }

            public class Cancel
            {
                [JsonProperty("status")]
                public string Status { get; set; }
            }

            public class Repost
            {
                [JsonProperty("status")]
                public string Status { get; set; }
            }

            public class EstimatedFee
            {
                [JsonProperty("fee")]
                public double Fee { get; set; }

                [JsonProperty("inputs")]
                public Output[] Inputs { get; set; }
            }
        }
    }
}

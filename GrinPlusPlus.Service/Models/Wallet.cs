using Newtonsoft.Json;
using static GrinPlusPlus.Service.Models.Basics;

namespace GrinPlusPlus.Service.Models
{
    public class Wallet
    {
        public class Balance
        {
            [JsonProperty("spendable")]
            public double Spendable { get; set; }

            [JsonProperty("total")]
            public double Total { get; set; }

            [JsonProperty("immature")]
            public double Immature { get; set; }

            [JsonProperty("unconfirmed")]
            public double Unconfirmed { get; set; }

            [JsonProperty("locked")]
            public double Locked { get; set; }
        }

        public class Address
        {
            [JsonProperty("status", Required = Required.Always)]
            public string Status { get; set; }


            [JsonProperty("tor_address")]
            public string TorAddress { get; set; }
        }

        public class Transactions
        {
            [JsonProperty("txs")]
            public Transaction[] List { get; set; }
        }

        public class Seed
        {
            [JsonProperty("wallet_seed")]
            public string MnemonicPhrase { get; set; }
        }
    }
}

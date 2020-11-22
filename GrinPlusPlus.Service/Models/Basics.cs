using Newtonsoft.Json;

namespace GrinPlusPlus.Service.Models
{
    public class Basics
    {
        public class Output
        {
            [JsonProperty("amount")]
            public double Amount { get; set; }

            [JsonProperty("block_height")]
            public int BlockHeight { get; set; }

            [JsonProperty("commitment")]
            public string Commitment { get; set; }

            [JsonProperty("keychain_path")]
            public string KeychainPath { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("label")]
            public string Label { get; set; }

            [JsonProperty("transaction_id")]
            public int TransactionId { get; set; }

            [JsonProperty("mmr_index")]
            public int Index { get; set; }
        }

        public class Kernel
        {
            [JsonProperty("commitment")]
            public string Commitment { get; set; }
        }

        public class Transaction
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("amount_credited")]
            public double AmountCredited { get; set; }

            [JsonProperty("amount_debited")]
            public double AmountDebited { get; set; }

            [JsonProperty("fee")]
            public double Fee { get; set; }

            [JsonProperty("confirmation_date_time")]
            public int ConfirmationDate { get; set; }

            [JsonProperty("creation_date_time")]
            public int CreationDate { get; set; }

            [JsonProperty("address")]
            public string Address { get; set; }

            [JsonProperty("slate_id")]
            public string Slate { get; set; }

            [JsonProperty("slate_message")]
            public string Message { get; set; }

            [JsonProperty("confirmed_height")]
            public int ConfirmedHeight { get; set; }

            [JsonProperty("kernels", Required = Required.AllowNull)]
            public Kernel[] Kernels { get; set; }

            [JsonProperty("outputs")]
            public Output[] Outputs { get; set; }
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    public class Owner
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Version { get; set; }

        private static class Endpoints
        {
            public const string Accounts = "accounts";
            public const string RetrieveOutputs = "retrieve_outputs";
        }

        private static string Token { get; set; }

        public void SeToken(string token)
        {
            Token = token;
        }

        Owner(string host = "127.0.0.1", int port = 3420)
        {
            Host = host;
            Port = port;
            Version = "v1/wallet/owner";
        }

        private static Owner instance = null;
        public static Owner Instance
        {
            get
            {
                if (instance == null)
                {
                    if (instance == null)
                    {
                        instance = new Owner("127.0.0.1", 3420);
                    }
                }
                return instance;
            }
        }

        public async Task<string[]> ListAccounts()
        {
            string url = $"http://{Host}:{Port}/{Version}/{Endpoints.Accounts}";
            HttpResponseMessage response = await (new HttpClient()).GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<string[]>(content);
            }

            throw new Exception("Error getting accounts");
        }

        public async Task<Models.Basics.Output[]> ListOutputs()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("session_token", Token);
            string url = $"http://{Host}:{Port}/{Version}/{Endpoints.RetrieveOutputs}";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Models.Basics.Output[]>(content);
            }

            throw new Exception("Error getting list of outputs");
        }

        public async Task<Models.Actions.Wallet.Open> OpenWallet(string username, string password, int seedLenght = 24)
        {
            var payload = new Dictionary<string, object>(){
                {"username", username},
                {"password", password},
            };

            return await GrinRPC.Request<Models.Actions.Wallet.Open>("login", payload);
        }

        public async Task<Models.Actions.Wallet.Create> CreateWallet(string username, string password, int seedLenght = 24)
        {
            var payload = new Dictionary<string, object>(){
                {"username", username},
                {"password", password},
                {"num_seed_words", seedLenght}
            };

            return await GrinRPC.Request<Models.Actions.Wallet.Create>("create_wallet", payload);
        }

        public async Task<Models.Actions.Wallet.Open> RestoreWallet(string username, string password, string seed)
        {
            var payload = new Dictionary<string, object>(){
                {"username", username},
                {"password", password},
                {"wallet_seed", seed}
            };

            return await GrinRPC.Request<Models.Actions.Wallet.Open>("restore_wallet", payload);
        }

        public async Task CloseWallet(string token)
        {
            var payload = new Dictionary<string, object>(){
                {"session_token", token},
            };

            await GrinRPC.Request("logout", payload);
        }

        public async Task<string> GetWalletSeed(string username, string password)
        {
            var payload = new Dictionary<string, object>(){
                {"username", username},
                {"password", password},
            };

            return await GrinRPC.Request<string>("get_wallet_seed", payload);
        }

        public async Task<Models.Basics.Transaction[]> GetWalletTransactions(string token, string[] statuses = null)
        {
            statuses = statuses ?? new string[]
            {
                "COINBASE", "SENT", "RECEIVED", "SENT_CANCELED", "RECEIVED_CANCELED",
                "SENDING_NOT_FINALIZED", "RECEIVING_IN_PROGRESS", "SENDING_FINALIZED"
            };

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                { "statuses", statuses }
            };

            var transactions = await GrinRPC.Request<Models.Wallet.Transactions>("list_txs", payload);

            if (transactions.List == null)
            {
                return new Models.Basics.Transaction[] { };
            }
            else
            {
                Array.Reverse(transactions.List);
            }

            return transactions.List;
        }

        public async Task<Models.Wallet.Balance> GetWalletBalance(string token)
        {
            var payload = new Dictionary<string, object>(){
                {"session_token", token},
            };

            return await GrinRPC.Request<Models.Wallet.Balance>("get_balance", payload);
        }

        public async Task<Models.Wallet.Address> GetWalletTorAddress(string token)
        {
            var payload = new Dictionary<string, object>(){
                {"session_token", token},
            };

            return await GrinRPC.Request<Models.Wallet.Address>("retry_tor", payload);
        }

        public async Task<Models.Actions.Transaction.EstimatedFee> EstimateTransactionFee(string token, double amount, string message = "", string strategy = "SMALLEST", string[] inputs = null)
        {
            if (inputs == null)
            {
                inputs = new string[] { };
            }

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"amount", amount * Math.Pow(10, 9) },
                {"fee_base", 1000000 },
                {"selection_strategy",
                    new Dictionary<string, object>(){
                        { "strategy", strategy },
                        { "inputs", strategy == "SMALLEST" ? new string[] { } : inputs },
                    }
                },
                {"message", message }
            };

            return await GrinRPC.Request<Models.Actions.Transaction.EstimatedFee>("estimate_fee", payload);
        }

        public async Task<Models.Actions.Coins.Send> SendCoins(string token, string address, double amount, string message = "",
                                                               string[] inputs = null, string strategy = "SMALLEST", bool max = false)
        {
            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"amount", amount * Math.Pow(10, 9) },
                {"fee_base", 1000000 },
                {"change_outputs ", 1 },
                {"selection_strategy",
                    new Dictionary<string, object>(){
                        { "strategy", strategy },
                        { "inputs", strategy == "SMALLEST"? new string[] { } : inputs},
                    }
                },
                {"message", message },
                {"address", address },
                {"post_tx", new Dictionary<string, object>(){
                        { "method", "STEM" },
                    }
                }
            };

            // the user wants to send the max amount
            if (max)
            {
                payload.Remove("amount");
                payload.Add("change_outputs ", 0);
            }

            return await GrinRPC.Request<Models.Actions.Coins.Send>("estimate_fee", payload);
        }

        public async Task<Models.Actions.Coins.Receive> ReceiveCoins(string token, string slatepack)
        {

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"slatepack", slatepack },
            };

            return await GrinRPC.Request<Models.Actions.Coins.Receive>("receive", payload);
        }

        public async Task<bool> FinalizeTransaction(string token, string slatepack)
        {

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"slatepack", slatepack },
                {"post_tx", new Dictionary<string, object>(){
                        { "method", "STEM" },
                    }
                }
            };

            var response = await GrinRPC.Request<Models.Actions.Transaction.Finalize>("finalize", payload);

            return response.Status.Trim().ToLower().Equals("finalized");
        }


        public async Task<bool> CancelTransaction(string token, int transactionId)
        {
            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"tx_id", transactionId}
            };

            var response = await GrinRPC.Request<Models.Actions.Transaction.Cancel>("finalized", payload);

            return response.Status.Trim().ToLower().Equals("success");
        }

        public async Task<bool> RepostTransaction(string token, int transactionId)
        {
            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"tx_id", transactionId},
                {"method", "FLUFF"} // FLUFF just for now
            };

            var response = await GrinRPC.Request<Models.Actions.Transaction.Repost>("repost_tx", payload);

            return response.Status.Trim().ToLower().Equals("success");
        }
    }
}

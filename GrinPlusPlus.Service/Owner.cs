using EdjCase.JsonRpc.Client;
using EdjCase.JsonRpc.Core;
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

        Owner(string host = "localhost", int port = 3420)
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
                        instance = new Owner("localhost", 3420);
                    }
                }
                return instance;
            }
        }

        private string RPCUrl
        {
            get
            {
                return "http://localhost:3421/v2";
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
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"username", username},
                {"password", password},
            };

            RpcRequest request = RpcRequest.WithParameterMap("login", payload, new RpcId(Guid.NewGuid().ToString()));
            RpcResponse<Models.Actions.Wallet.Open> response = await client.SendRequestAsync<Models.Actions.Wallet.Open>(request);

            if (!response.HasError)
            {
                return response.Result;
            }

            throw new Exception(response.Error.Message);
        }

        public async Task<Models.Actions.Wallet.Create> CreateWallet(string username, string password, int seedLenght = 24)
        {
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"username", username},
                {"password", password},
                {"num_seed_words", seedLenght}
            };

            RpcRequest request = RpcRequest.WithParameterMap("create_wallet", payload, new RpcId(Guid.NewGuid().ToString()));
            RpcResponse<Models.Actions.Wallet.Create> response =
                await client.SendRequestAsync<Models.Actions.Wallet.Create>(request);

            if (!response.HasError)
            {
                return response.Result;
            }

            throw new Exception(response.Error.Message);
        }

        public async Task<Models.Actions.Wallet.Open> RestoreWallet(string username, string password, string seed)
        {
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"username", username},
                {"password", password},
                {"wallet_seed", seed}
            };


            RpcRequest request = RpcRequest.WithParameterMap("restore_wallet", payload, new RpcId(Guid.NewGuid().ToString()));
            RpcResponse<Models.Actions.Wallet.Open> response = await client.SendRequestAsync<Models.Actions.Wallet.Open>(request);

            if (!response.HasError)
            {
                return response.Result;
            }

            throw new Exception(response.Error.Message);
        }

        public async Task<bool> CloseWallet(string token)
        {
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"username", token},
            };

            RpcRequest request = RpcRequest.WithParameterMap("logout", payload, new RpcId(Guid.NewGuid().ToString()));
            var response = await client.SendRequestAsync(request);

            if (!response.HasError)
            {
                return true;
            }

            throw new Exception(response.Error.Message);
        }

        public async Task<string> GetWalletSeed(string username, string password)
        {
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"username", username},
                {"password", password},
            };

            RpcRequest request = RpcRequest.WithParameterMap("get_wallet_seed", payload,
                new RpcId(Guid.NewGuid().ToString()));
            RpcResponse<string> response = await client.SendRequestAsync<string>(request);

            if (!response.HasError)
            {
                return response.Result;
            }

            throw new Exception(response.Error.Message);
        }

        public async Task<Models.Basics.Transaction[]> GetWalletTransactions(string token, string[] statuses = null)
        {
            statuses = statuses ?? new string[]
            {
                "COINBASE", "SENT", "RECEIVED", "SENT_CANCELED", "RECEIVED_CANCELED",
                "SENDING_NOT_FINALIZED", "RECEIVING_IN_PROGRESS", "SENDING_FINALIZED"
            };
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                { "statuses", statuses }
            };

            RpcRequest request = RpcRequest.WithParameterMap("list_txs", payload,
                new RpcId(Guid.NewGuid().ToString()));
            RpcResponse response = await client.SendRequestAsync(request);

            if (!response.HasError)
            {
                string content = ((string)response.Result).Trim();
                if (!string.IsNullOrEmpty(content))
                {
                    Models.Wallet.Transactions history =
                    JsonConvert.DeserializeObject<Models.Wallet.Transactions>(content,
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    });
                    if (history.List == null)
                    {
                        return new Models.Basics.Transaction[] { };
                    }
                    Array.Reverse(history.List);
                    return history.List;
                }
            }

            throw new Exception(response.Error.Message);
        }

        public async Task<Models.Wallet.Balance> GetWalletBalance(string token)
        {
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
            };

            RpcRequest request = RpcRequest.WithParameterMap("get_balance", payload,
                new RpcId(Guid.NewGuid().ToString()));
            RpcResponse<Models.Wallet.Balance> response = await client.SendRequestAsync<Models.Wallet.Balance>(request);

            if (!response.HasError)
            {
                return response.Result;
            }

            throw new Exception(response.Error.Message);
        }

        public async Task<Models.Wallet.Address> GetWalletTorAddress(string token)
        {
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
            };

            RpcRequest request = RpcRequest.WithParameterMap("retry_tor", payload,
                new RpcId(Guid.NewGuid().ToString()));
            RpcResponse response =
                await client.SendRequestAsync(request);

            if (!response.HasError)
            {
                return JsonConvert.DeserializeObject<Models.Wallet.Address>((string)response.Result,
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    });
            }

            throw new Exception(response.Error.Message);
        }

        public async Task<Models.Actions.Transaction.EstimatedFee> EstimateTransactionFee(string token, double amount, string message = "", string strategy = "SMALLEST", string[] inputs = null)
        {
            if (inputs == null)
            {
                inputs = new string[] { };
            }
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"amount", amount * Math.Pow(10, 9) },
                {"fee_base", 1000000 },
                {"selection_strategy",
                    new Dictionary<string, object>(){
                        { "strategy", strategy },
                        { "inputs", strategy == "SMALLEST" && inputs.Length > 0? new string[] { } : inputs },
                    }
                },
                {"message", message }
            };

            RpcRequest request = RpcRequest.WithParameterMap("estimate_fee", payload,
                new RpcId(Guid.NewGuid().ToString()));
            RpcResponse response = await client.SendRequestAsync(request);

            if (!response.HasError)
            {
                string content = ((string)response.Result).Trim();
                if (!string.IsNullOrEmpty(content))
                {
                    Models.Actions.Transaction.EstimatedFee estimation =
                    JsonConvert.DeserializeObject<Models.Actions.Transaction.EstimatedFee>(content);
                    return estimation;
                }
            }

            throw new Exception(response.Error.Message);
        }

        public async Task<Models.Actions.Coins.Send> SendCoins(string token, string address, double amount, string strategy, string[] inputs, string message)
        {
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"amount", amount * Math.Pow(10, 9) },
                {"fee_base", 1000000 },
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

            RpcRequest request = RpcRequest.WithParameterMap("send", payload,
                new RpcId(Guid.NewGuid().ToString()));
            RpcResponse response = await client.SendRequestAsync(request);

            if (!response.HasError)
            {
                string content = ((string)response.Result).Trim();
                if (!string.IsNullOrEmpty(content))
                {
                    return JsonConvert.DeserializeObject<Models.Actions.Coins.Send>(content);
                }
            }

            throw new Exception(response.Error.Message);
        }

        public async Task<Models.Actions.Coins.Receive> ReceiveCoins(string token, string slatepack)
        {
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"slatepack", slatepack },
                {"post_tx", new Dictionary<string, object>(){
                        { "method", "STEM" },
                    }
                }
            };

            RpcRequest request = RpcRequest.WithParameterMap("send", payload,
                new RpcId(Guid.NewGuid().ToString()));
            RpcResponse response = await client.SendRequestAsync(request);

            if (!response.HasError)
            {
                string content = ((string)response.Result).Trim();
                if (!string.IsNullOrEmpty(content))
                {
                    return JsonConvert.DeserializeObject<Models.Actions.Coins.Receive>(content);
                }
            }

            throw new Exception(response.Error.Message);
        }

        public async Task<bool> FinalizeTransaction(string token, string slatepack)
        {
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"slatepack", slatepack },
                {"post_tx", new Dictionary<string, object>(){
                        { "method", "STEM" },
                    }
                }
            };

            RpcRequest request = RpcRequest.WithParameterMap("send", payload,
                new RpcId(Guid.NewGuid().ToString()));
            RpcResponse response = await client.SendRequestAsync(request);

            if (!response.HasError)
            {
                string content = ((string)response.Result).Trim();
                if (!string.IsNullOrEmpty(content))
                {
                    Models.Actions.Transaction.Finalize finalizeResponse =
                        JsonConvert.DeserializeObject<Models.Actions.Transaction.Finalize>(content);
                    return finalizeResponse.Status.Trim().ToLower().Equals("finalized");
                }
            }

            throw new Exception(response.Error.Message);
        }


        public async Task<bool> CancelTransaction(string token, int transactionId)
        {
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"tx_id", transactionId}
            };

            RpcRequest request = RpcRequest.WithParameterMap("cancel_tx", payload,
                new RpcId(Guid.NewGuid().ToString()));
            RpcResponse<Models.Actions.Transaction.Cancel> response =
                await client.SendRequestAsync<Models.Actions.Transaction.Cancel>(request);

            if (!response.HasError)
            {
                return response.Result.Status.Trim().ToLower().Equals("success");
            }

            throw new Exception(response.Error.Message);
        }

        public async Task<bool> RepostTransaction(string token, int transactionId)
        {
            RpcClient client = new RpcClient(new Uri(RPCUrl));

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"tx_id", transactionId},
                {"method", "FLUFF"} // FLUFF just for now
            };

            RpcRequest request = RpcRequest.WithParameterMap("repost_tx", payload,
                new RpcId(Guid.NewGuid().ToString()));

            RpcResponse<Models.Actions.Transaction.Repost> response =
                await client.SendRequestAsync<Models.Actions.Transaction.Repost>(request);

            if (!response.HasError)
            {
                return response.Result.Status.Trim().ToLower().Equals("success");
            }

            throw new Exception(response.Error.Message);
        }
    }
}

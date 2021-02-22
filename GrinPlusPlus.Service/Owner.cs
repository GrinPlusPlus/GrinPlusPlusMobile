using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    public class Owner
    {
        private static class Endpoints
        {
            public const string Accounts = "list_wallets";
            public const string RetrieveOutputs = "retrieve_outputs";
            public const string Login = "login";
            public const string CreateWallet = "create_wallet";
            public const string RestoreWallet = "restore_wallet";
            public const string GetWalletSeed = "get_wallet_seed";
            public const string DeleteWallet = "delete_wallet";
            public const string TransactionsList = "list_txs";
            public const string GetWalletBalance = "get_balance";
            public const string EstimateFee = "estimate_fee";
            public const string RepostTransaction = "repost_tx";
            public const string CancelTransaction = "cancel_tx";
            public const string FinalizeTransaction = "finalize";
            public const string ReceiveTransaction = "receive";
            public const string SendTransaction = "send";
        }

        Owner()
        {
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
                        instance = new Owner();
                    }
                }
                return instance;
            }
        }

        public async Task<Models.Basics.Accounts> ListAccounts()
        {
            var payload = new Dictionary<string, object>() { };

            return await GrinOwnerRPC.Request<Models.Basics.Accounts>(Endpoints.Accounts, payload);
        }

        public async Task<Models.Basics.Output[]> ListOutputs(string token)
        {
            return await GrinOwnerAPI.Request<Models.Basics.Output[]>(Endpoints.RetrieveOutputs, new Dictionary<string, string>() { { "session_token", token } });
        }

        public async Task<Models.Actions.Wallet.Open> OpenWallet(string username, string password)
        {
            var payload = new Dictionary<string, object>(){
                {"username", username},
                {"password", password},
            };

            return await GrinOwnerRPC.Request<Models.Actions.Wallet.Open>(Endpoints.Login, payload);
        }

        public async Task<Models.Actions.Wallet.Create> CreateWallet(string username, string password, int seedLenght = 24)
        {
            var payload = new Dictionary<string, object>(){
                {"username", username},
                {"password", password},
                {"num_seed_words", seedLenght}
            };

            return await GrinOwnerRPC.Request<Models.Actions.Wallet.Create>(Endpoints.CreateWallet, payload);
        }

        public async Task<Models.Actions.Wallet.Open> RestoreWallet(string username, string password, string seed)
        {
            var payload = new Dictionary<string, object>(){
                {"username", username},
                {"password", password},
                {"wallet_seed", seed}
            };

            return await GrinOwnerRPC.Request<Models.Actions.Wallet.Open>(Endpoints.RestoreWallet, payload);
        }

        public async Task CloseWallet(string token)
        {
            var payload = new Dictionary<string, object>(){
                {"session_token", token},
            };

            await GrinOwnerRPC.Request("logout", payload);
        }

        public async Task<string> GetWalletSeed(string username, string password)
        {
            var payload = new Dictionary<string, object>(){
                {"username", username},
                {"password", password},
            };

            var seed = await GrinOwnerRPC.Request<Models.Wallet.Seed>(Endpoints.GetWalletSeed, payload);

            return seed.MnemonicPhrase;
        }

        public async Task<bool> DeleteWallet(string username, string password)
        {
            var payload = new Dictionary<string, object>(){
                {"username", username},
                {"password", password},
            };

            var response = await GrinOwnerRPC.Request<Models.Actions.Wallet.Delete>(Endpoints.DeleteWallet, payload);
            return response.Status.Trim().ToLower().Equals("success");
        }

        public async Task<Models.Basics.Transaction[]> GetWalletTransactions(string token, string[] statuses = null)
        {
            statuses = statuses ?? new string[]
            {
                "COINBASE", "SENT", "RECEIVED", "SENT_CANCELED",
                "RECEIVED_CANCELED","SENDING_NOT_FINALIZED","RECEIVING_IN_PROGRESS",
                "SENDING_FINALIZED"
            };

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                { "statuses", statuses }
            };

            var transactions = await GrinOwnerRPC.Request<Models.Wallet.Transactions>(Endpoints.TransactionsList, payload);

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

            return await GrinOwnerRPC.Request<Models.Wallet.Balance>(Endpoints.GetWalletBalance, payload);
        }

        public async Task<Models.Actions.Transaction.EstimatedFee> EstimateTransactionFee(string token, double amount, string message = "",
            string strategy = "SMALLEST", string[] inputs = null)
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

            return await GrinOwnerRPC.Request<Models.Actions.Transaction.EstimatedFee>(Endpoints.EstimateFee, payload);
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

            return await GrinOwnerRPC.Request<Models.Actions.Coins.Send>(Endpoints.SendTransaction, payload);
        }

        public async Task<Models.Actions.Coins.Receive> ReceiveCoins(string token, string slatepack)
        {

            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"slatepack", slatepack },
            };

            return await GrinOwnerRPC.Request<Models.Actions.Coins.Receive>(Endpoints.ReceiveTransaction, payload);
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

            var response = await GrinOwnerRPC.Request<Models.Actions.Transaction.Finalize>(Endpoints.FinalizeTransaction, payload);

            return response.Status.Trim().ToLower().Equals("finalized");
        }


        public async Task<bool> CancelTransaction(string token, int transactionId)
        {
            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"tx_id", transactionId}
            };

            var response = await GrinOwnerRPC.Request<Models.Actions.Transaction.Cancel>(Endpoints.CancelTransaction, payload);

            return response.Status.Trim().ToLower().Equals("success");
        }

        public async Task<bool> RepostTransaction(string token, int transactionId)
        {
            var payload = new Dictionary<string, object>(){
                {"session_token", token},
                {"tx_id", transactionId},
                {"method", "FLUFF"} // FLUFF just for now
            };

            var response = await GrinOwnerRPC.Request<Models.Actions.Transaction.Repost>(Endpoints.RepostTransaction, payload);

            return response.Status.Trim().ToLower().Equals("success");
        }
    }
}

using GrinPlusPlus.Models;
using MihaZupan;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrinPlusPlus.Api
{
    class GrinPPLocalService : IDataProvider
    {
        public async Task<Login> CreateWallet(string username, string password, int seedLength)
        {
            var wallet = await Service.Owner.Instance.CreateWallet(username, password, seedLength);
            return new Login()
            {
                Token = wallet.Token,
                SlatepackAdddress = wallet.SlatepackAdddress,
                TorAdddress = wallet.TorAdddress
            };
        }

        public async Task<Login> DoLogin(string username, string password)
        {
            var login = await Service.Owner.Instance.OpenWallet(username, password);

            if (login == null)
            {
                throw new Exception("Unable to login");
            }

            return new Login()
            {
                Token = login.Token,
                SlatepackAdddress = login.SlatepackAdddress,
                TorAdddress = login.TorAdddress
            };
        }

        public async Task<List<Account>> GetAccounts()
        {
            List<Account> accounts = new List<Account>();
            foreach (var account in await Service.Owner.Instance.ListAccounts())
            {
                accounts.Add(new Account() { Name = account });
            }
            return accounts;
        }

        public async Task<List<Transaction>> GetTransactions(string token, string[] statuses)
        {
            List<Transaction> transactions = new List<Transaction>();

            foreach (var transaction in await Service.Owner.Instance.GetWalletTransactions(token, statuses))
            {
                List<Kernel> kernels = new List<Kernel>();
                if (transaction.Kernels != null)
                {
                    foreach (var kernel in transaction.Kernels)
                    {
                        kernels.Add(new Kernel()
                        {
                            Commitment = kernel.Commitment
                        });
                    }
                }

                List<Output> outputs = new List<Output>();
                if (transaction.Outputs != null)
                {
                    foreach (var output in transaction.Outputs)
                    {
                        outputs.Add(new Output()
                        {
                            Commitment = output.Commitment,
                            Amount = output.Amount,
                            BlockHeight = output.BlockHeight,
                            KeychainPath = output.KeychainPath,
                            Label = output.Label,
                            Status = output.Status,
                            Index = output.Index,
                        });
                    }
                }

                transactions.Add(new Transaction()
                {
                    Id = transaction.Id,
                    Status = transaction.Type,
                    AmountCredited = transaction.AmountCredited,
                    AmountDebited = transaction.AmountDebited,
                    Fee = transaction.Fee,
                    Date = DateTimeOffset.FromUnixTimeSeconds(transaction.CreationDate).UtcDateTime,
                    Slate = transaction.Slate,
                    Address = transaction.Address,
                    Message = transaction.Message,
                    ConfirmedHeight = transaction.ConfirmedHeight,
                    Kernels = kernels,
                    Outputs = outputs,
                });
            }

            return transactions;
        }

        public async Task<Balance> GetWalletBalance(string token)
        {
            var balance = await Service.Owner.Instance.GetWalletBalance(token);
            return new Balance()
            {
                Spendable = balance.Spendable,
                Immature = balance.Immature,
                Locked = balance.Locked,
                Unconfirmed = balance.Unconfirmed,
                Total = balance.Total,
            };
        }

        public async Task<Login> RestoreWallet(string username, string password, string seed)
        {
            var wallet = await Service.Owner.Instance.RestoreWallet(username, password, seed);
            return new Login()
            {
                Token = wallet.Token,
                SlatepackAdddress = wallet.SlatepackAdddress,
                TorAdddress = wallet.TorAdddress
            };
        }

        public async Task<FeeEstimation> EstimateFee(string token, double amount, string message = "", string strategy = "SMALLEST", string[] inputs = null)
        {
            if (inputs == null)
            {
                inputs = new string[] { };
            }
            var estimation = await Service.Owner.Instance.EstimateTransactionFee(token, amount, message, strategy, inputs);
            var estimationInputs = new List<Output>();
            foreach (var input in estimation.Inputs)
            {
                estimationInputs.Add(new Output()
                {
                    Amount = input.Amount,
                    BlockHeight = input.BlockHeight,
                    Index = input.Index,
                    Status = input.Status,
                    Label = input.Label,
                    Commitment = input.Commitment,
                    KeychainPath = input.KeychainPath,
                    TransactionId = input.TransactionId
                });
            }
            return new FeeEstimation() { Fee = estimation.Fee, Inputs = estimationInputs };
        }

        public async Task<SendingResponse> SendGrins(string token, string address, double amount, string message = "", string[] inputs = null, string strategy = "SMALLEST", bool max = false)
        {
            var response = await Service.Owner.Instance.SendCoins(token, address, amount, message, inputs, strategy, max);
            return new SendingResponse() { Error = response.Error, Slatepack = response.Slatepack, Status = response.Status };
        }

        public async Task<bool> CancelTransaction(string token, int transaction)
        {
            return await Service.Owner.Instance.CancelTransaction(token, transaction);
        }

        public async Task<ReceivingResponse> ReceiveTransaction(string token, string slatepack)
        {
            var response = await Service.Owner.Instance.ReceiveCoins(token, slatepack);
            return new ReceivingResponse() { Error = response.Error, Slatepack = response.Slatepack, Status = response.Status };
        }

        public async Task<bool> FinalizeTransaction(string token, string slatepack)
        {
            return await Service.Owner.Instance.FinalizeTransaction(token, slatepack);
        }

        public async Task<bool> RepostTransaction(string token, int transaction)
        {
            return await Service.Owner.Instance.RepostTransaction(token, transaction);
        }

        public async Task<List<Peer>> GetNodeConnectedPeers()
        {
            var peers = new List<Peer>();
            foreach (var peer in await Service.Node.Instance.ConnectedPeers())
            {
                peers.Add(new Peer() { Address = peer.Address, Agent = peer.Agent, Direction = peer.Direction });
            }
            return peers;
        }

        public async Task<NodeStatus> GetNodeStatus()
        {
            var nodeStatus = await Service.Node.Instance.Status();
            return new NodeStatus() { 
                Chain = new Chain { 
                    Hash = nodeStatus.Chain.Hash,
                    Height = nodeStatus.Chain.Height,
                    PreviousHash = nodeStatus.Chain.PreviousHash,
                    Difficulty = nodeStatus.Chain.Difficulty,
                },
                HeaderHeight = nodeStatus.HeaderHeight,
                Network = new Network { 
                    Height = nodeStatus.Network.Height,
                    Inbound = nodeStatus.Network.Inbound,
                    Outbound = nodeStatus.Network.Outbound,
                    Difficulty = nodeStatus.Network.Difficulty,
                },
                ProtocolVersion = nodeStatus.ProtocolVersion,
                State = new State()
                {
                    DownloadSize = nodeStatus.State.DownloadSize,
                    Downloaded = nodeStatus.State.Downloaded,
                    ProcessingStatus = nodeStatus.State.ProcessingStatus,
                },
                SyncStatus = nodeStatus.SyncStatus,
                Agent = nodeStatus.Agent
            };
        }

        public async Task<bool> CheckAddressAvailability(string address)
        {
            var proxy = new HttpToSocks5Proxy("127.0.0.1", 3422);
            var handler = new HttpClientHandler { Proxy = proxy };

            var url = "http://grinchck.ahcbagldgzdpa74g2mh74fvk5zjzpfjbvgqin6g3mfuu66tynv2gkiid.onion/check/";
            var parameters = new Dictionary<string, string> { { "wallet", address } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var httpclient = new HttpClient(handler, true);
            
            var response = await httpclient.PostAsync(url, encodedContent).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return (await response.Content.ReadAsStringAsync()).Trim().ToLower().Equals("reachable");
            }
            return false;
        }

        public async Task DoLogout(string token)
        {
            await Service.Owner.Instance.CloseWallet(token);
        }
    }
}

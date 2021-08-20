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
        public async Task<string> BackupWallet(string username, string password)
        {
            return await Service.Owner.Instance.GetWalletSeed(username, password).ConfigureAwait(false);
        }

        public async Task<Login> CreateWallet(string username, string password, int seedLength)
        {
            var wallet = await Service.Owner.Instance.CreateWallet(username, password, seedLength).ConfigureAwait(false);
            return new Login()
            {
                Token = wallet.Token,
                SlatepackAdddress = wallet.SlatepackAdddress,
                TorAddress = wallet.TorAdddress,
                ListenerPort = wallet.Listener,
                Seed = wallet.Seed
            };
        }

        public async Task<bool> DeleteWallet(string username, string password)
        {
            return await Service.Owner.Instance.DeleteWallet(username, password).ConfigureAwait(false);
        }

        public async Task<Login> DoLogin(string username, string password)
        {
            var login = await Service.Owner.Instance.OpenWallet(username, password).ConfigureAwait(false);

            if (login == null)
            {
                throw new Exception("Unable to login");
            }

            return new Login()
            {
                Token = login.Token,
                SlatepackAdddress = login.SlatepackAdddress,
                TorAddress = login.TorAdddress,
                ListenerPort = login.Listener
            };
        }

        public async Task<List<Account>> GetAccounts()
        {
            List<Account> accounts = new List<Account>();
            foreach (var account in (await Service.Owner.Instance.ListAccounts().ConfigureAwait(false)).Wallets)
            {
                accounts.Add(new Account() { Name = account });
            }
            return accounts;
        }

        public async Task<List<Transaction>> GetTransactions(string token, string[] statuses)
        {
            List<Transaction> transactions = new List<Transaction>();

            foreach (var transaction in await Service.Owner.Instance.GetWalletTransactions(token, statuses).ConfigureAwait(false))
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

                // Unix timestamp is seconds past epoch
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(transaction.CreationDate).ToLocalTime();

                transactions.Add(new Transaction()
                {
                    Id = transaction.Id,
                    Status = transaction.Type,
                    AmountCredited = transaction.AmountCredited,
                    AmountDebited = transaction.AmountDebited,
                    Fee = transaction.Fee,
                    Date = dtDateTime,
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
            var balance = await Service.Owner.Instance.GetWalletBalance(token).ConfigureAwait(false);
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
            var wallet = await Service.Owner.Instance.RestoreWallet(username, password, seed).ConfigureAwait(false);
            return new Login()
            {
                Token = wallet.Token,
                SlatepackAdddress = wallet.SlatepackAdddress,
                TorAddress = wallet.TorAdddress
            };
        }

        public async Task<FeeEstimation> EstimateFee(string token, double amount, string message = "", string strategy = "SMALLEST", string[] inputs = null)
        {
            if (inputs == null)
            {
                inputs = new string[] { };
            }
            var estimation = await Service.Owner.Instance.EstimateTransactionFee(token, amount, message, strategy, inputs).ConfigureAwait(false);
            var estimationInputs = new List<Output>();
            return new FeeEstimation() { Fee = estimation.Fee, Inputs = estimationInputs };
        }

        public async Task<SendingResponse> SendGrins(string token, string address, double amount, string message = "", string[] inputs = null, string strategy = "SMALLEST", bool max = false)
        {
            var response = await Service.Owner.Instance.SendCoins(token, address, amount, message, inputs, strategy, max).ConfigureAwait(false);
            return new SendingResponse() { Error = response.Error, Slatepack = response.Slatepack, Status = response.Status };
        }

        public async Task<bool> CancelTransaction(string token, int transaction)
        {
            return await Service.Owner.Instance.CancelTransaction(token, transaction).ConfigureAwait(false);
        }

        public async Task<ReceivingResponse> ReceiveTransaction(string token, string slatepack)
        {
            var response = await Service.Owner.Instance.ReceiveCoins(token, slatepack).ConfigureAwait(false);
            return new ReceivingResponse() { Error = response.Error, Slatepack = response.Slatepack, Status = response.Status };
        }

        public async Task<bool> FinalizeTransaction(string token, string slatepack)
        {
            return await Service.Owner.Instance.FinalizeTransaction(token, slatepack).ConfigureAwait(false);
        }

        public async Task<bool> RepostTransaction(string token, int transaction)
        {
            return await Service.Owner.Instance.RepostTransaction(token, transaction).ConfigureAwait(false);
        }

        public async Task<List<Peer>> GetNodeConnectedPeers()
        {
            var peers = new List<Peer>();
            foreach (var peer in await Service.Node.Instance.ConnectedPeers().ConfigureAwait(false))
            {
                peers.Add(new Peer() { Address = peer.Address, Agent = peer.Agent, Direction = peer.Direction });
            }
            return peers;
        }

        public async Task<NodeStatus> GetNodeStatus()
        {
            var nodeStatus = await Service.Node.Instance.Status().ConfigureAwait(false);
            return new NodeStatus()
            {
                Chain = new Chain
                {
                    Hash = nodeStatus.Chain.Hash,
                    Height = (ulong)nodeStatus.Chain.Height,
                    PreviousHash = nodeStatus.Chain.PreviousHash,
                    Difficulty = nodeStatus.Chain.Difficulty,
                },
                HeaderHeight = (ulong)nodeStatus.HeaderHeight,
                Network = new Network
                {
                    Height = (ulong)nodeStatus.Network.Height,
                    Inbound = nodeStatus.Network.Inbound,
                    Outbound = nodeStatus.Network.Outbound,
                    Difficulty = nodeStatus.Network.Difficulty,
                },
                ProtocolVersion = nodeStatus.ProtocolVersion,
                State = new State()
                {
                    DownloadSize = (ulong)nodeStatus.State.DownloadSize,
                    Downloaded = (ulong)nodeStatus.State.Downloaded,
                    ProcessingStatus = nodeStatus.State.ProcessingStatus,
                },
                SyncStatus = nodeStatus.SyncStatus,
                Agent = nodeStatus.Agent
            };
        }

        public async Task<bool> CheckAddressAvailability(string address, string api = null)
        {
            var proxy = new HttpToSocks5Proxy("127.0.0.1", 3422);
            var handler = new HttpClientHandler { Proxy = proxy };

            var url = api ?? "http://grinchck.ahcbagldgzdpa74g2mh74fvk5zjzpfjbvgqin6g3mfuu66tynv2gkiid.onion/check/";

            var destination = $"http://{address}.onion";

            var parameters = new Dictionary<string, string> { { "wallet", destination } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var httpclient = new HttpClient(handler, true)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            var response = await httpclient.PostAsync(url, encodedContent).ConfigureAwait(false);

            var content = string.Empty;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                content = (await response.Content.ReadAsStringAsync()).Trim().ToLower();
            }
            return content.Equals("reachable");
        }

        public async Task DoLogout(string token)
        {
            await Service.Owner.Instance.CloseWallet(token);
        }

        public async Task UpdateNodeSettings(int MinimumPeers, int MaximumPeers, int Confirmations)
        {
            await Service.Foreign.Instance.UpdateSettings(MinimumPeers, MaximumPeers, Confirmations).ConfigureAwait(false);
        }

        public async Task<NodePreferences> GetNodeSettings()
        {
            var settings = await Service.Foreign.Instance.GetSettings().ConfigureAwait(false);
            return new NodePreferences()
            {
                MinimumPeers = settings.MinimumPeers,
                MaximumPeers = settings.MaximumPeers,
                Confirmations = settings.Confirmations
            };
        }
    }
}

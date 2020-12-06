using GrinPlusPlus.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrinPlusPlus.Api
{
    public interface IDataProvider
    {
        Task<Login> CreateWallet(string username, string password, int seedLength);
        
        Task<string> BackupWallet(string username, string password);

        Task<bool> DeleteWallet(string username, string password);
        
        Task<Login> DoLogin(string username, string password);

        Task<List<Account>> GetAccounts();

        Task<List<Transaction>> GetTransactions(string token, string[] statuses);

        Task<Balance> GetWalletBalance(string token);

        Task<Login> RestoreWallet(string username, string password, string seed);

        Task<FeeEstimation> EstimateFee(string token, double amount, string message = "", string strategy = "SMALLEST", string[] inputs = null);

        Task<SendingResponse> SendGrins(string token, string address, double amount, string message = "", string[] inputs = null, string strategy = "SMALLEST", bool max = false);

        Task<ReceivingResponse> ReceiveTransaction(string token, string slatepack);

        Task DoLogout(string token);

        Task<List<Peer>> GetNodeConnectedPeers();

        Task<NodeStatus> GetNodeStatus();

        Task<bool> FinalizeTransaction(string token, string slatepack);

        Task<bool> CancelTransaction(string token, int transaction);

        Task<bool> RepostTransaction(string token, int transaction);

        Task<bool> CheckAddressAvailability(string address);

        Task DeleteONION(string torAddress);

        Task AddONION();
    }
}

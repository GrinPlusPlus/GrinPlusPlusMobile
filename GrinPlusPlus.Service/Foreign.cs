using EdjCase.JsonRpc.Client;
using EdjCase.JsonRpc.Core;
using System;
using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    public class Foreign
    {
        public string Version { get; set; }

        private static class Endpoints
        {
            public const string CheckVersion = "check_version";
            public const string Receive = "receive_tx";
            public const string Finalize = "finalize_tx";
        }

        Foreign()
        {
            Version = "v2/foreign`";
        }

        private static Foreign instance = null;
        public static Foreign Instance
        {
            get
            {
                if (instance == null)
                {
                    if (instance == null)
                    {
                        instance = new Foreign();
                    }
                }
                return instance;
            }
        }

        public async Task<string> CheckVersion(string address, string slate)
        {
            RpcClient client = new RpcClient(new Uri($"{address}${Version}"));

            var payload = new[] { slate };

            RpcRequest request = RpcRequest.WithParameterList(Endpoints.CheckVersion, payload,
                new RpcId(Guid.NewGuid().ToString()));
            RpcResponse<string> response = await client.SendRequestAsync<string>(request);

            return response.Result;
        }

        public async Task<string> ReceiveTransaction(string address, string slate)
        {
            RpcClient client = new RpcClient(new Uri($"{address}${Version}"));

            var payload = new[] { slate };

            RpcRequest request = RpcRequest.WithParameterList(Endpoints.Receive, payload,
                new RpcId(Guid.NewGuid().ToString()));
            RpcResponse<string> response = await client.SendRequestAsync<string>(request);

            return response.Result;
        }

        public async Task<string> FinalizeTransaction(string address, string slate)
        {
            RpcClient client = new RpcClient(new Uri($"{address}${Version}"));

            var payload = new[] { slate };

            RpcRequest request = RpcRequest.WithParameterList(Endpoints.Finalize, payload,
                new RpcId(Guid.NewGuid().ToString()));
            RpcResponse<string> response = await client.SendRequestAsync<string>(request);

            return response.Result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    public class Foreign
    {
        private static class Endpoints
        {
            public const string GetSettings = "get_config";
            public const string UpdateSettings = "update_config";
        }

        Foreign()
        {
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

        public async Task UpdateSettings(int minPeers, int maxPeers, int confirmations)
        {
            var payload = new Dictionary<string, object>(){
                {"max_peers", maxPeers},
                {"min_peers", minPeers},
                {"min_confirmations", confirmations}
            };

            await GrinForeignRPC.Request(Endpoints.UpdateSettings, payload);
        }

        public async Task<Models.Node.Settings> GetSettings()
        {
            return await GrinForeignRPC.Request<Models.Node.Settings>(Endpoints.GetSettings);
        }
    }
}

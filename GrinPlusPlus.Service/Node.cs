using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    public class Node
    {
        private static class Endpoints
        {
            public const string Status = "status";
            public const string Resync = "resync";
            public const string ConnectedPeers = "peers/connected";
            public const string Shutdown = "shutdown";
        }

        Node() { }

        private static Node instance = null;
        public static Node Instance
        {
            get
            {
                if (instance == null)
                {
                    if (instance == null)
                    {
                        instance = new Node();
                    }
                }
                return instance;
            }
        }

        public async Task<Models.Node.Status> Status()
        {
            return await GrinAPI.Request<Models.Node.Status>(Endpoints.Status).ConfigureAwait(false);
        }

        public async Task<Models.Node.Peer[]> ConnectedPeers()
        {
            return await GrinAPI.Request<Models.Node.Peer[]>(Endpoints.ConnectedPeers);
        }

        public async Task Resync()
        {
            await GrinAPI.Request(Endpoints.Resync).ConfigureAwait(false);
        }

        public async Task Shutdown()
        {
            await GrinAPI.Request(Endpoints.Shutdown).ConfigureAwait(false);
        }
    }
}

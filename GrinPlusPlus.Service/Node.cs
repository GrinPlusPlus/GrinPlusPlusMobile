using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrinPlusPlus.Service
{
    public class Node
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Version { get; set; }

        private static class Endpoints
        {
            public const string Status = "status";
            public const string Resync = "resync";
            public const string ConnectedPeers = "peers/connected";
            public const string Shutdown = "shutdown";
        }

        Node(string host = "localhost", int port = 3413) : base()
        {
            Host = host;
            Port = port;
            Version = "v1";
        }

        private static Node instance = null;
        public static Node Instance
        {
            get
            {
                if (instance == null)
                {
                    if (instance == null)
                    {
                        instance = new Node("localhost", 3413);
                    }
                }
                return instance;
            }
        }

        public async Task<Models.Node.Status> Status()
        {
            Models.Node.Status status = new Models.Node.Status();
            try
            {
                string url = $"http://{Host}:{Port}/{Version}/{Endpoints.Status}";
                HttpResponseMessage response = await new HttpClient().GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    status = JsonConvert.DeserializeObject<Models.Node.Status>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return status;
        }

        public async Task<Models.Node.Peer[]> ConnectedPeers()
        {
            Models.Node.Peer[] peers = new Models.Node.Peer[] { };
            try
            {
                string url = $"http://{Host}:{Port}/{Version}/{Endpoints.ConnectedPeers}";
                HttpResponseMessage response = await new HttpClient().GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    peers = JsonConvert.DeserializeObject<Models.Node.Peer[]>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return peers;
        }

        public async Task Resync()
        {
            string url = $"http://{Host}:{Port}/{Version}/{Endpoints.Resync}";
            await (new HttpClient()).PostAsync(url, null);
        }

        public async Task Shutdown()
        {
            string url = $"http://{Host}:{Port}/{Version}/{Endpoints.Shutdown}";
            await (new HttpClient()).PostAsync(url, null);
        }
    }
}

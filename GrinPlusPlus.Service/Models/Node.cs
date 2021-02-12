using Newtonsoft.Json;


namespace GrinPlusPlus.Service.Models
{
    public class Node
    {
        public class Settings
        {
            [JsonProperty("min_peers")]
            public int MinimumPeers { get; set; }

            [JsonProperty("max_peers")]
            public int MaximumPeers { get; set; }

            [JsonProperty("min_confirmations")]
            public int Confirmations { get; set; }
        }


        public class Chain
        {
            [JsonProperty("hash")]
            public string Hash { get; set; }

            [JsonProperty("height")]
            public int Height { get; set; }

            [JsonProperty("previous_hash")]
            public string PreviousHash { get; set; }

            [JsonProperty("total_difficulty")]
            public ulong Difficulty { get; set; }
        }

        public class Network
        {
            [JsonProperty("height")]
            public int Height { get; set; }

            [JsonProperty("num_inbound")]
            public int Inbound { get; set; }

            [JsonProperty("num_outbound")]
            public int Outbound { get; set; }

            [JsonProperty("total_difficulty")]
            public ulong Difficulty { get; set; }
        }

        public class State
        {
            [JsonProperty("download_size")]
            public int DownloadSize { get; set; }

            [JsonProperty("downloaded")]
            public int Downloaded { get; set; }

            [JsonProperty("processing_status")]
            public int ProcessingStatus { get; set; }
        }

        public class Status
        {
            [JsonProperty("chain")]
            public Chain Chain { get; set; }

            [JsonProperty("header_height")]
            public int HeaderHeight { get; set; }

            [JsonProperty("network")]
            public Network Network { get; set; }

            [JsonProperty("protocol_version")]
            public int ProtocolVersion { get; set; }

            [JsonProperty("state")]
            public State State { get; set; }

            [JsonProperty("sync_status")]
            public string SyncStatus { get; set; }

            [JsonProperty("user_agent")]
            public string Agent { get; set; }
        }

        public class Capabilities
        {
            [JsonProperty("bits")]
            public int Bits { get; set; }
        }
        public class Peer
        {
            [JsonProperty("addr")]
            public string Address { get; set; }

            [JsonProperty("ban_reason")]
            public string BanReason { get; set; }

            [JsonProperty("capabilities")]
            public Capabilities Capabilities { get; set; }

            [JsonProperty("direction")]
            public string Direction { get; set; }

            [JsonProperty("flags")]
            public string Flags { get; set; }

            [JsonProperty("height")]
            public int Height { get; set; }

            [JsonProperty("last_banned")]
            public int LastBanned { get; set; }

            [JsonProperty("last_connected")]
            public int LastConnected { get; set; }

            [JsonProperty("protocol_version")]
            public int ProtocolVersion { get; set; }

            [JsonProperty("total_difficulty")]
            public ulong Difficulty { get; set; }

            [JsonProperty("user_agent")]
            public string Agent { get; set; }
        }
    }
}

using System;

namespace GrinPlusPlus.Models
{
    public class Chain
    {
        public string Hash { get; set; }

        public ulong Height { get; set; }

        public string PreviousHash { get; set; }

        public ulong Difficulty { get; set; }
    }

    public class Network
    {
        public ulong Height { get; set; }

        public int Inbound { get; set; }

        public int Outbound { get; set; }

        public ulong Difficulty { get; set; }
    }

    public class State
    {
        public ulong DownloadSize { get; set; }

        public ulong Downloaded { get; set; }

        public int ProcessingStatus { get; set; }
    }

    public class NodeStatus
    {
        public Chain Chain { get; set; }

        public ulong HeaderHeight { get; set; }

        public Network Network { get; set; }

        public int ProtocolVersion { get; set; }

        public State State { get; set; }

        private string _syncStatus;
        public string SyncStatus
        {
            get
            {
                switch (_syncStatus)
                {
                    case "FULLY_SYNCED":
                        return "Running";
                    case "SYNCING_HEADERS":
                        return "1/4 Syncing Headers";
                    case "DOWNLOADING_TXHASHSET":
                        return "2/4 Downloading State";
                    case "PROCESSING_TXHASHSET":
                        return "3/4 Validating State";
                    case "SYNCING_BLOCKS":
                        return "4/4 Syncing Blocks";
                    case "NOT_CONNECTED":
                        return "Waiting for Peers";
                    default:
                        return "Not Connected";
                }
            }
            set { _syncStatus = value; }
        }

        public string Agent { get; set; }
        public double ProgressPercentage
        {
            get
            {
                switch (_syncStatus)
                {
                    case "SYNCING_HEADERS":
                        return Helpers.GetFraction(HeaderHeight, Network.Height);
                    case "DOWNLOADING_TXHASHSET":
                        return Helpers.GetFraction(State.Downloaded, State.DownloadSize);
                    case "SYNCING_BLOCKS":
                        if (HeaderHeight < 10080)
                        {
                            return Helpers.GetFraction(Chain.Height, HeaderHeight);
                        }
                        return Helpers.GetFraction(10080 - Math.Min(10080, HeaderHeight - Chain.Height), 10080);
                    case "PROCESSING_TXHASHSET":
                        return Helpers.GetFraction((ulong)State.ProcessingStatus, 100);
                    default:
                        return 0;
                }
            }
        }

    }
}

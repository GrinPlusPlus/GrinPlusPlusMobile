namespace GrinPlusPlus.Models
{
    public class Chain
    {
        public string Hash { get; set; }

        public int Height { get; set; }

        public string PreviousHash { get; set; }

        public ulong Difficulty { get; set; }
    }

    public class Network
    {
        public int Height { get; set; }

        public int Inbound { get; set; }

        public int Outbound { get; set; }

        public ulong Difficulty { get; set; }
    }

    public class State
    {
        public int DownloadSize { get; set; }

        public int Downloaded { get; set; }

        public int ProcessingStatus { get; set; }
    }

    public class NodeStatus
    {
        public Chain Chain { get; set; }

        public int HeaderHeight { get; set; }

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
                int numerator;
                int denominator;

                if (_syncStatus.Equals("SYNCING_HEADERS"))
                {
                    numerator = HeaderHeight;
                    denominator = Network.Height;

                    return (double)(((float)numerator / (float)denominator));
                }
                else if (_syncStatus.Equals("DOWNLOADING_TXHASHSET"))
                {
                    numerator = State.Downloaded;
                    denominator = State.DownloadSize;

                    return (float)numerator / (float)denominator;
                }
                else if (_syncStatus.Equals("SYNCING_BLOCKS"))
                {
                    numerator = Network.Height - Chain.Height;
                    denominator = 10080;
                    return ((double)(((float)numerator / (float)denominator))) / 100;
                }

                else if (_syncStatus.Equals("PROCESSING_TXHASHSET"))
                {
                    return State.ProcessingStatus / 100;
                }

                return 0;
            }
        }

    }
}

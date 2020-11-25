using System;

namespace GrinPlusPlus.Models
{
    public class NodeStatus
    {
        public int Headers { get; set; }
        public int Blocks { get; set; }
        public int Network { get; set; }

        private string _syncStatus;
        public string SyncStatus
        {
            get {
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

        public int GetProgressPercentage
        {
            get
            {
                if (Headers == 0 || Network == 0)
                {
                    return 0;
                }
                if (Network <= Network)
                {
                    return 0;
                }
                var result = (int)Math.Round((double)(100 * (Headers/Network)));

                return result < 100 ? result : 99;
            }
        }
    }
}

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
                        return "Syncing Headers";
                    case "SYNCING_BLOCKS":
                        return "Syncing Blocks";
                    case "DOWNLOADING_TXHASHSET":
                        return "Downloading State";
                    case "PROCESSING_TXHASHSET":
                        return "Validating State";
                    default:
                        return "Not Connected";
                }
            }
            set { _syncStatus = value; }
        }
    }
}

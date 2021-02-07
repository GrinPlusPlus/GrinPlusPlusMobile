using System;

namespace GrinPlusPlus.Service
{
    public static class SyncHelpers
    {
        public static double GetFraction(ulong numerator, ulong denominator)
        {
            if (numerator == 0 || denominator == 0) return 0;
            if (denominator <= 0)
            {
                return 0;
            }
            return numerator / (float)denominator;
        }

        public static string GetStatusLabel(string status)
        {
            switch (status)
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
                    return "Disconnected";
            }
        }

        public static double GetProgressPercentage(Models.Node.Status status)
        {
            switch (status.SyncStatus)
            {
                case "FULLY_SYNCED":
                    return 1;
                case "SYNCING_HEADERS":
                    return GetFraction((ulong)status.HeaderHeight, (ulong)status.Network.Height);
                case "DOWNLOADING_TXHASHSET":
                    return GetFraction((ulong)status.State.Downloaded, (ulong)status.State.DownloadSize);
                case "SYNCING_BLOCKS":
                    if (status.HeaderHeight < 10080)
                    {
                        return GetFraction((ulong)status.Chain.Height, (ulong)status.HeaderHeight);
                    }
                    return GetFraction(10080 - Math.Min(10080, (ulong)status.HeaderHeight - (ulong)status.Chain.Height), 10080);
                case "PROCESSING_TXHASHSET":
                    return GetFraction((ulong)status.State.ProcessingStatus, 100);
                default:
                    return 0;
            }
        }
    }
}

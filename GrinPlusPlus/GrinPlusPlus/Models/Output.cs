using System;

namespace GrinPlusPlus.Models
{
    public class Output
    {
        private double _amount;
        public double Amount
        {
            get { return _amount / Math.Pow(10, 9); }
            set { _amount = value; }
        }

        public int BlockHeight { get; set; }

        public string Commitment { get; set; }

        public string KeychainPath { get; set; }

        public string Label { get; set; }

        public string Status { get; set; }

        public int TransactionId { get; set; }

        public int Index { get; set; }
    }
}

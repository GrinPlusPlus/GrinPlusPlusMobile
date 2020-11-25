using System;
using System.Collections.Generic;

namespace GrinPlusPlus.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public double AmountCredited { get; set; }
        public double AmountDebited { get; set; }

        private double _fee;
        public double Fee
        {
            get { return _fee / Math.Pow(10, 9); }
            set { _fee = value; }
        }
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public string Slate { get; set; }
        public string Message { get; set; }
        public int ConfirmedHeight { get; set; }
        public List<Kernel> Kernels { get; set; }
        public List<Output> Outputs { get; set; }

        public string Amount
        {
            get
            {
                return Helpers.GetInteger(Math.Abs(AmountCredited - AmountDebited));
            }
        }

        public string Decimals
        {
            get
            {
                return Helpers.GetDecimals(Math.Abs(AmountCredited - AmountDebited));
            }
        }

        public bool CanBeFinalized
        {
            get
            {
                return Status.ToLower().Equals("sending (not finalized)");
            }
        }

        public bool CanBeReposted
        {
            get
            {
                return Status.ToLower().Equals("sending (finalized)");
            }
        }

        public bool CanBeCanceled
        {
            get
            {
                return Status.ToLower().Equals("receiving (unconfirmed)") ||
                        Status.ToLower().Equals("sending (not finalized)");
            }
        }
    }
}

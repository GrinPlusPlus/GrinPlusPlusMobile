using System;

namespace GrinPlusPlus.Models
{
    public class Balance
    {
        public double Spendable { get; set; }

        public double Total { get; set; }

        public double Immature { get; set; }

        public double Unconfirmed { get; set; }

        public double Locked { get; set; }

        public string SpendableInteger
        {
            get
            {
                string d = (Spendable / Math.Pow(10, 9)).ToString();
                if (d.IndexOf(".") > -1)
                {
                    d = d.Substring(0, d.IndexOf("."));
                }
                return d.Replace(".", "");
            }
        }

        public string SpendableDecimals
        {
            get
            {
                string d = (Spendable / Math.Pow(10, 9)).ToString();
                if (d.IndexOf(".") > -1)
                {
                    d = d.Substring(d.IndexOf("."));
                    return d.PadRight(9, '0').Replace(".", "");
                }
                return ".".PadRight(10, '0').Replace(".", "");
            }
        }

        public string ImmatureInteger
        {
            get
            {
                string d = (Immature / Math.Pow(10, 9)).ToString();
                if (d.IndexOf(".") > -1)
                {
                    d = d.Substring(0, d.IndexOf("."));
                }
                return d.Replace(".", "");
            }
        }

        public string ImmatureDecimals
        {
            get
            {
                string d = (Immature / Math.Pow(10, 9)).ToString();
                if (d.IndexOf(".") > -1)
                {
                    d = d.Substring(d.IndexOf("."));
                    return d.PadRight(9, '0').Replace(".", "");
                }
                return ".".PadRight(10, '0').Replace(".", "");
            }
        }

        public string UnconfirmedInteger
        {
            get
            {
                string d = (Unconfirmed / Math.Pow(10, 9)).ToString();
                if (d.IndexOf(".") > -1)
                {
                    d = d.Substring(0, d.IndexOf("."));
                }
                return d.Replace(".", "");
            }
        }
        public string UnconfirmedDecimals
        {
            get
            {
                string d = (Unconfirmed / Math.Pow(10, 9)).ToString();
                if (d.IndexOf(".") > -1)
                {
                    d = d.Substring(d.IndexOf("."));
                    return d.PadRight(9, '0').Replace(".", "");
                }
                return ".".PadRight(10, '0').Replace(".", "");
            }
        }

        public string LockedInteger
        {
            get
            {
                string d = (Locked / Math.Pow(10, 9)).ToString();
                if (d.IndexOf(".") > -1)
                {
                    d = d.Substring(0, d.IndexOf("."));
                }
                return d.Replace(".", "");
            }
        }
        public string LockedDecimals
        {
            get
            {
                string d = (Locked / Math.Pow(10, 9)).ToString();
                if (d.IndexOf(".") > -1)
                {
                    d = d.Substring(d.IndexOf("."));
                    return d.PadRight(9, '0').Replace(".", "");
                }
                return ".".PadRight(10, '0').Replace(".", "");
            }
        }
    }
}

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
                return Helpers.GetInteger(Spendable);
            }
        }

        public string SpendableDecimals
        {
            get
            {
                return Helpers.GetDecimals(Spendable);
            }
        }

        public string ImmatureInteger
        {
            get
            {
                return Helpers.GetInteger(Immature);
            }
        }

        public string ImmatureDecimals
        {
            get
            {
                return Helpers.GetDecimals(Immature);
            }
        }

        public string UnconfirmedInteger
        {
            get
            {
                return Helpers.GetInteger(Unconfirmed);
            }
        }
        public string UnconfirmedDecimals
        {
            get
            {
                return Helpers.GetDecimals(Unconfirmed);
            }
        }

        public string LockedInteger
        {
            get
            {
                return Helpers.GetInteger(Locked);
            }
        }
        public string LockedDecimals
        {
            get
            {
                return Helpers.GetDecimals(Locked);
            }
        }
    }
}

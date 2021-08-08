using System.Collections.Generic;

namespace GrinPlusPlus.Models
{
    public class TransactionGroup : List<Transaction>
    {
        public string Name { get; private set; }

        public TransactionGroup(string name, List<Transaction> transactions) : base(transactions)
        {
            Name = name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

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

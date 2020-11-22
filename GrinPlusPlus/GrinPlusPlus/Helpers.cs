using GrinPlusPlus.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GrinPlusPlus
{
    public static class Helpers
    {
        public static string[] GetTransactionsListGroups(List<Transaction> transactions)
        {
            var groups = new List<string>() { };
            foreach(Transaction transaction in transactions)
            {
                string date = transaction.Date.ToString();
                groups.Add(date);
            }
            return groups.Distinct().ToArray();
        }
    }
}

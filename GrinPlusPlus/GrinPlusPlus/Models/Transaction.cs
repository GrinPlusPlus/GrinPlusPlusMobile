using System;
using System.Collections.Generic;

namespace GrinPlusPlus.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public double Decimals { get; set; }
        public double Fee { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public string Slate { get; set; }
        public string Message { get; set; }
        public string Kernels { get; set; }
        public List<Output> Outputs { get; set; }
    }
}

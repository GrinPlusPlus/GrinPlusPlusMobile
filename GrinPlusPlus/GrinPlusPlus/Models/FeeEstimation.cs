using System;
using System.Collections.Generic;
using System.Text;

namespace GrinPlusPlus.Models
{
    public class FeeEstimation
    {
        public double Fee { get; set; }

        public List<Output> Inputs { get; set; }
    }
}

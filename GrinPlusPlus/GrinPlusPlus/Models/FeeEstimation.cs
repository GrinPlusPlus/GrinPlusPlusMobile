using System.Collections.Generic;

namespace GrinPlusPlus.Models
{
    public class FeeEstimation
    {
        public double Fee { get; set; }

        public List<Output> Inputs { get; set; }
    }
}

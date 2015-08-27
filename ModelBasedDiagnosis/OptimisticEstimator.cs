using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    class OptimisticEstimator:BatchCostEstimator
    {
        public OptimisticEstimator()
        {
            Overhead = 10;
        }
        public override double StateCost(SystemState state)
        {
            return Overhead;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    class PessimisticEstimator:BatchCostEstimator
    {
        public PessimisticEstimator()
        {
            Overhead = 10;
        }
        public override double StateCost(SystemState state) //esuming the systemState is already without R (the next)
        {
            double ans = 0;
            if (state == null || state.HelthState == null || state.HelthState.Count == 0)
                return ans;
            foreach (double h in state.HelthState.CurrentHelthState)
            {
                ans += h;
            }
            ans = ans * Overhead;
            return ans;
        }
    }
}

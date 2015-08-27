using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    abstract class BatchCostEstimator//:ICostEstimator
    {
        public double Overhead { get; set; }

        public double WastedCostUtility(List<Gate> repairAction, SystemState state)
        {
            if (state == null || repairAction == null)
                return 0;
            return WastedCost(repairAction,state.HelthState) + (1 - state.SystemRepair(repairAction)) * StateCost(state);
        }
        public double ComputeRepairCost(List<Gate> repairAction)
        {
            double ans = 0;
            if (repairAction == null || repairAction.Count == 0)
                return ans;
            foreach (Gate g in repairAction)
            {
                if (g != null)
                    ans += g.Cost;
            }
            ans += Overhead;
            return ans;
        }

        public double WastedCost(List<Gate> repairAction,HelthStateVector helthState)
        {
            double ans = 0;
            if (repairAction == null || repairAction.Count == 0 || helthState == null || helthState.Count == 0)
                return ans;
            foreach (Gate g in repairAction)
            {
                ans += ((1- helthState.GetCompHealthState(g)) * g.Cost);
            }
            return ans;
        }

        public abstract double StateCost(SystemState state);
        
    }
}

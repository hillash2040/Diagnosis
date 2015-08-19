using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    abstract class BatchFutureCostEstimator:ICostEstimator
    {
        public double Overhead { get; set; }

        /*public double ComputeCost(List<Gate> repairAction, DiagnosisSet diagnoses)
        {
            double ans = 0;
            if (repairAction != null && repairAction.Count > 0 && diagnoses != null && diagnoses.Count > 0)
                ans = WastedCost(repairAction) + (1 - SystemRepair(repairAction, diagnoses)) * StateCost(diagnoses);
            return ans;
            return WastedCost(repairAction) + (1 - SystemRepair(repairAction, diagnoses)) * StateCost(diagnoses);
        }*/
        public double RepairActionCost(List<Gate> repairAction, SystemState state)
        {
            return WastedCost(repairAction,state.HelthState) + (1 - SystemRepair(repairAction, state.Diagnoses)) * StateCost(diagnoses);
        }
        public double SystemRepair(List<Gate> repairAction, DiagnosisSet diagnoses) //changes
        {
            double sysRepair = 0;
            if (diagnoses == null || diagnoses.Count == 0 || repairAction==null||repairAction.Count==0)
                return sysRepair;
            foreach (Diagnosis diag in diagnoses.Diagnoses)
            {
                if (diag.TheDiagnosis == null || diag.TheDiagnosis.Count ==0 || diag.TheDiagnosis.Count > repairAction.Count)
                    continue;
                double p = diag.Probability;
                foreach (Gate g in diag.TheDiagnosis)
                {
                    if (!repairAction.Contains(g))
                    {
                        p = 0;
                        break;
                    }
                        
                }
                sysRepair += p;
            }
            return sysRepair;
        }

        public double WastedCost(List<Gate> repairAction,HelthStateVector helthState)
        {
            //to do
            double ans = 0;
            return ans;
        }

        public abstract double StateCost(SystemState state);
        
    }
}

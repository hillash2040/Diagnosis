using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    interface ICostEstimator
    {
        double RepairActionCost(List<Gate> repairAction, SystemState state);
        double StateCost(SystemState state);
    }
}

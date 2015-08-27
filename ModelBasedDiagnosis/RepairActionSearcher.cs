using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    abstract class RepairActionSearcher
    {
       public abstract List<List<Gate>> ComputePossibleAcions(SystemState state,int k);
    }
}

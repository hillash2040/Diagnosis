using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    class MDPNode
    {
        public SystemState State;
        public List<List<Gate>> Actions; // what we repaired so far
        
    }
}

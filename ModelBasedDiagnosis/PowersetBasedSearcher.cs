using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    class PowersetBasedSearcher:RepairActionSearcher
    {
        public override List<List<Gate>> ComputePossibleAcions(SystemState state, int k)
        {
            if(state==null|| state.Diagnoses ==null || state.Diagnoses.Count==0)
                return null;
            List<List<Gate>> ans = new List<List<Gate>>();
            List<Gate> comps = new List<Gate>(); // hash set?
            foreach (Diagnosis diag in state.Diagnoses.Diagnoses)
            {
                foreach (Gate g in diag.TheDiagnosis)
                {
                    if (!comps.Contains(g))
                    {
                        comps.Add(g);
                        List<Gate> list = new List<Gate>();
                        list.Add(g);
                        ans.Add(list);
                    }
                       
                }
            }
            while (k > 1)
            {
                List<List<Gate>> temp = new List<List<Gate>>();
                foreach (List<Gate> list in ans)
                {
                    foreach (Gate g in comps)
                    {
                        if (!list.Contains(g))
                        {
                            List<Gate> newList = new List<Gate>(list);
                            newList.Add(g);
                            temp.Add(newList);
                        }
                    }
                }
                ans.AddRange(temp);
                k--;
            }
            return ans;
        }
    }
}

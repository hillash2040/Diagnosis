using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    class UnionBasedSearcher :RepairActionSearcher
    {
        private int mostleft;
        public override List<List<Gate>> ComputePossibleAcions(SystemState state, int k)
        {
            if (state == null || state.Diagnoses == null || state.Diagnoses.Count == 0)
                return null;
            bool[] bitVector = new bool[state.Diagnoses.Count];
            mostleft = -1;
            List<List<Gate>> ans = new List<List<Gate>>();

            Trie<List<Gate>> trie = new Trie<List<Gate>>();
           // UInt16 g = new UInt16();
          //  int f = g | 1;
            
            return ans;
            //to do
        }

        public bool[] NextPermutation(bool[] bitVector)
        {
            bool[] ans = new bool[bitVector.Length];
            int i = mostleft + 1;
            if(i>=bitVector.Length)
            {

            }
            else if (bitVector[i])
            {

            }
            else
            {
                ans[i] = true;
            }
          /*  for (int i = 0; i < bitVector.Length; i++)
            {
                if (!bitVector[i])
                {
                    if(i==bitVector.Length-1)
                    {
                        ans[0] = true;
                        break;
                    }
                    continue;
                }
                int j = i+1;
                while (j<bitVector.Length&&bitVector[j])
                {

                }
                ans[j] = true;
                    
            }*/
            
        }
            
    }
}

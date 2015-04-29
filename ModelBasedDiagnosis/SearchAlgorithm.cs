using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    abstract class SearchAlgorithm :IDiagnoser
    {
        public IFunction function;
        protected Trie<string> trie;
        protected Observation observation;
        protected List<int> closed;
        protected TimeSpan x;
        protected List<double> hSVector;
        protected int diagnosesCounter;
        //public bool FindMinCard;

        public SearchAlgorithm(IFunction function, TimeSpan timeSpan): this(function)
        {
            x = timeSpan;
        }
        public SearchAlgorithm(IFunction function)
        {
            this.function = function;
            closed = new List<int>();
            x = new TimeSpan(0, 1, 0); 
        }
        public SearchAlgorithm() :this(null)
        {}
        public void addToGoodCompList(int compID)
        {
            closed.Add(compID);
        }
        public void resetGoodCompList()
        {
            closed.Clear();
        }
        public abstract List<Diagnosis> FindDiagnoses(Observation observation);
        public abstract bool Stop();
        protected double CalcHSVectorDistance(List<double> newHSVector)
        {
            double ans = 0;
            if (hSVector == null || hSVector.Count == 0 || newHSVector == null || hSVector.Count != newHSVector.Count)
                return -1;
            for (int i = 0; i < newHSVector.Count; i++)
            {
                double j = Math.Pow((newHSVector[i] - hSVector[i]), 2);
                ans += j;
            }
            ans = Math.Sqrt(ans);
            return ans;
        }
        public List<double> CalcHelthState(List<Gate> components,List<Diagnosis> diagnoses)
        {
            List<double> compHS = new List<double>(components.Count); //check if the init val is 0
            foreach (Diagnosis diag in diagnoses)
            {
                foreach (Gate comp in diag.diagnosis)
                {
                    int i = components.IndexOf(comp);
                    if (i >= 0)
                    {
                        if (compHS[i] >= 0)
                            compHS[i] += diag.probability;
                        else
                            compHS[i] = diag.probability;
                    }
                }
            }
            return compHS;
        }
        public void CalcDiagProb(List<Diagnosis> diagnoses)
        {
            if(diagnoses==null|| diagnoses.Count==0)
                return;
            double sum = 0;
            foreach (Diagnosis diag in diagnoses)
            {
               /* double x = 1;
                foreach (Gate g in diag)
                {
                    x = x * g.P;
                } diagProb.Add(x);
                sum += x;*/ 
                double x = 0.01; //if we change the prob for each comp to be diff - delete the part below and put the part above
                double s = Math.Pow(x, diag.diagnosis.Count);
                diag.ChangeProbability(s);
                sum += s;
               
            }
            foreach (Diagnosis diag in diagnoses)
            {
                diag.ChangeProbability(diag.probability / sum);
            }
        }
        protected bool isDamaged()
        {
            bool[] modelOutput = observation.TheModel.GetValue();
            for (int i = 0; i < observation.OutputValues.Length; i++)
            {
                if (modelOutput[i] != observation.OutputValues[i])
                    return false;
            }
            return true;
        }
        protected bool isInTrie(List<Gate> list)//for CreateSet
        {
            if (list == null || list.Count == 0)
                return false;
            bool ans = true;
            if (list.Count == 1)
            {
                Gate g = list.First();
                foreach (char c in g.Id + "")
                {
                    if (!trie.Matcher.NextMatch(c))
                    {
                        ans = false;
                        break;
                    }
                }
                if (ans)
                    ans = trie.Matcher.IsExactMatch();
                while (trie.Matcher.LastMatch() != 0 && trie.Matcher.LastMatch() != 32)
                {
                    trie.Matcher.BackMatch();
                }
                return ans;
            }
            List<Gate> temp = new List<Gate>(list);
            foreach (Gate g in list)
            {
                ans = true;
                temp.RemoveAt(0);
                foreach (char c in g.Id + "")
                {
                    if (!trie.Matcher.NextMatch(c))
                    {
                        ans = false;
                        break;//
                    }
                }
                if (ans)
                {
                    if (trie.Matcher.NextMatch(' '))
                    {
                        ans = isInTrie(temp);
                        trie.Matcher.BackMatch();
                    }
                }
                //backtrack
                while (trie.Matcher.LastMatch() != 0 && trie.Matcher.LastMatch() != 32)
                {
                    trie.Matcher.BackMatch();
                }
                if (ans)
                    break;

            }
            return ans;

        }
        
    }
}

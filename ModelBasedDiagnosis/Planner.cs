using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    class Planner
    {
        public enum Type {random,highestProb,bestDiag}
        private Type type;
        public List<int> fixedComp;
        public double Overhead { get; set; }
        public Planner()
        {
            fixedComp = new List<int>();
            Overhead = 10; ///////////
        }
        public Planner(Type type)
        {
            this.type = type;
            fixedComp = new List<int>();
            Overhead = 10;///////////
        }
        public Gate Plan(List<List<Gate>> diagnosis)
        {
            Gate ans = null;
            switch (type)
            {
                case Type.random:
                    ans= Random(diagnosis);
                    break;
                case Type.highestProb:
                    ans= HighstProb(diagnosis);
                    break;
                case Type.bestDiag:
                    ans= BestDiagnosis(diagnosis);
                    break;
            }
            if (ans != null)
                fixedComp.Add(ans.Id);
            return ans;
        }
        public List<Gate> Demo(List<List<Gate>> diagnosis)
        {
            List<double> diagProb = getDiagsProb(diagnosis);
            if (diagProb == null || diagProb.Count != diagnosis.Count)
            {
                Console.WriteLine("Planner Fail");
                return null;
            }
            Random r = new Random();
            double val = r.NextDouble();
            int index = 0;
            double sum = 0;
            foreach (double d in diagProb)
            {
                if (sum < val)
                {
                    sum += d;
                    if (sum >= val)
                        break;
                }
                else
                    break;
                index++;
            }
            if (index < 0 || index >= diagnosis.Count)
                index = 0; //!!!!!!!!!
            return diagnosis[index];
        }
     /*   private Gate EvaluationBased(List<List<Gate>> diagnosis)
        {
            //calc wasted cost foreach diag
            //futuresingle
            List<double> wastedCost = new List<double>();
            List<double> futureSingle = new List<double>();
            foreach (List<Gate> diag in diagnosis)
            {
                double wc =0;
                double fs = 0;
                foreach(Gate g in diag)
                {
                    wc += (1 - g.H) * g.Cost;
                    fs += (1 - g.H) * Overhead;
                }
                wastedCost.Add(wc);
                futureSingle.Add(wc + fs);
            }

            //futurebacth
            List<double> diagProb = getDiagsProb(diagnosis);
            List<double> futureBatch = new List<double>();
            for (int i = 0; i < diagnosis.Count; i++)
            {
                double eval = diagProb[i];
                for (int j = 0; j < diagnosis.Count; j++)
                {
                    if (i == j)
                        continue;
                    foreach (Gate g in diagnosis[i])
                    {
                        if (diagnosis[j].Contains(g)) 
                        {
                            eval += diagProb[i];
                            break;
                        }
                    }
                }
                eval = (eval * Overhead) + wastedCost[i];
                futureBatch.Add(eval);
            }

            Gate ans=null;
            //find minimal from batch and single
            //choose gate
            return ans;
        }*/
        private Gate Random(List<List<Gate>> diagnosis)
        {
            Random r = new Random();
            int index = r.Next(0, diagnosis.Count - 1);
            List<Gate> d = diagnosis[index];
            index = r.Next(0, d.Count - 1);
            return d[index]; 
        }
        private Gate HighstProb(List<List<Gate>> diagnosis)
        {
            Gate ans;
            List<double> diagProb = getDiagsProb(diagnosis);
            if (diagProb == null || diagProb.Count != diagnosis.Count)
            {
                Console.WriteLine("Planner Fail");
                return null;
            }
            Dictionary<Gate, double> compHS = getCompHelthState(diagnosis, diagProb);
            if (compHS.Count == 0)
                return null;
            double max = 0;
            ans = compHS.First().Key;
            foreach (Gate g in compHS.Keys)
            {
                if (compHS[g] > max)
                {
                    max = compHS[g];
                    ans = g;
                }

            }
            return ans;
        }
        private Gate BestDiagnosis(List<List<Gate>> diagnosis)
        {
            Gate ans;
            List<Gate> bd = getBD(diagnosis);
            if (bd == null || bd.Count == 0)
                return null;
            Random r = new Random();
            int j = r.Next(bd.Count - 1);
            ans = bd[j]; 
            return ans;
        }
        protected List<Gate> getBD(List<List<Gate>> diagnosis)
        {
            List<double> diagProb = getDiagsProb(diagnosis);
            if (diagProb == null || diagProb.Count != diagnosis.Count)
            {
                //Console.WriteLine("Planner Fail");
                return null;
            }
            double max = 0;
            List<Gate> bd = diagnosis[0];
            for (int i = 0; i < diagnosis.Count; i++)
            {
                if (diagProb[i] > max)
                {
                    max = diagProb[i];
                    bd = diagnosis[i];
                }
            }
            return bd;
        }
        protected Dictionary<Gate, double> getCompHelthState(List<List<Gate>> diagnosis, List<double> diagProb)
        {
            Dictionary<Gate, double> compHS = new Dictionary<Gate, double>();

            for (int i = 0; i < diagnosis.Count; i++)
            {
                foreach (Gate g in diagnosis[i])
                {
                    if (!compHS.ContainsKey(g))
                    {
                        compHS.Add(g, 0);
                    }
                    compHS[g] += diagProb[i];
                }
            }

            return compHS;
        }
        protected List<double> getDiagsProb(List<List<Gate>> diagnosis)
        {
            List<double> diagProb = new List<double>();
            double sum = 0;
            foreach (List<Gate> diag in diagnosis)
            {
               /* double x = 1;
                foreach (Gate g in diag)
                {
                    x = x * g.P;
                } diagProb.Add(x);
                sum += x;*/ 
                double x = 0.01; //if we change the prob for each comp to be diff - delete the part below and put the part above
                double s = Math.Pow(x, diag.Count);
                diagProb.Add(s);
                sum += s;
               
            }
            for (int i = 0; i < diagProb.Count; i++)
            {
                diagProb[i] = diagProb[i] / sum;
            }
            return diagProb;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    class BatchPlanner : Planner
    {
        public enum BacthType { batchHighestProb, batchBestDiag, evalSingleSubset, evalSingleUnion, evalBatchSubset, evalBatchUnion }
        private BacthType type;
        private int K;
        private double Min;
        public BatchPlanner()
        {
            K = 1;
        }
        public BatchPlanner(BacthType type, int k)
        {
            this.type = type;
            if (k < 1)
                K = 1;
            else
                K = k;
        }
        public List<Gate> Plan(List<List<Gate>> diagnosis)
        {
            Min = -1;
            if (type == BacthType.batchBestDiag)
                return getBD(diagnosis);
            List<double> diagProb = getDiagsProb(diagnosis);
            if (diagProb == null || diagProb.Count == 0)
                return null;
            Dictionary<Gate, double> compHS = getCompHelthState(diagnosis, diagProb);
            if (compHS == null || compHS.Count == 0)
                return null;
            if (type == BacthType.batchHighestProb)
                return BatchHP(diagnosis, compHS);
            List<List<Gate>> workingSet = new List<List<Gate>>();
            List<Gate> ans = null;
            double min = 0;
            List<Gate> compList = new List<Gate>();
            if (type == BacthType.evalBatchSubset || type == BacthType.evalSingleSubset)
            {
                foreach (Gate g in compHS.Keys)
                {
                    compList.Add(g);
                    List<Gate> temp = new List<Gate>();
                    temp.Add(g);
                    double eval;
                    double sysRepair = calcSysRepair(temp, diagnosis, diagProb);
                    if (type == BacthType.evalBatchSubset)
                        eval = FutureBatch(temp, compHS, sysRepair);
                    else
                        eval = FutureSingle(temp, compHS, sysRepair);
                    if (ans == null || eval < min)
                    {
                        min = eval;
                        ans = temp;
                    }
                    workingSet.Add(temp);
                }
            }

            else if (type == BacthType.evalBatchUnion || type == BacthType.evalSingleUnion)
            {
                double prob = 0;
                for (int i = 0; i < diagnosis.Count; i++)
                {
                    List<Gate> diag = diagnosis[i];
                    prob += diagProb[i];
                    workingSet.Add(diag);
                    double eval;
                    double sysRepair = calcSysRepair(diag, diagnosis, diagProb);
                    if (type == BacthType.evalBatchUnion)
                        eval = FutureBatch(diag, compHS, sysRepair);
                    else
                        eval = FutureSingle(diag, compHS, sysRepair);
                    if (ans == null || eval < min)
                    {
                        min = eval;
                        ans = diag;
                    }
                    if (prob >= 0.7)
                        break;
                }
            }


            if (K > 1)
            {
                foreach (List<Gate> list in workingSet)
                {
                    Min = -1;
                    List<Gate> tempAns = null;
                    if (type == BacthType.evalBatchSubset)
                        tempAns = evalBatchSubset(list, compHS, diagnosis, diagProb, compList);
                    else if (type == BacthType.evalSingleSubset)
                        tempAns = evalSingleSubset(list, compHS, compList, diagnosis, diagProb);
                    else if (type == BacthType.evalBatchUnion)
                        tempAns = evalBatchUnion(list, compHS, diagnosis, diagProb, 1);
                    else if (type == BacthType.evalSingleUnion)
                        tempAns = evalSingleUnion(list, compHS, diagnosis, diagProb, 1);
                    if (tempAns != null && Min != -1 && Min < min)
                    {
                        ans = tempAns;
                        min = Min;
                    }
                }
            }

            return ans;
        }
        private List<Gate> BatchHP(List<List<Gate>> diagnosis, Dictionary<Gate, double> compHS)
        {
            List<Gate> ans = new List<Gate>();
            if (compHS.Keys.Count <= K)
            {
                foreach (Gate g in compHS.Keys)
                    ans.Add(g);
                return ans;
            }
            int i = 1;
            while (i <= K)
            {
                double max = 0;
                Gate toAdd = null;
                foreach (Gate g in compHS.Keys)
                {
                    if (ans.Contains(g))
                        continue;
                    if (compHS[g] > max)
                    {
                        max = compHS[g];
                        toAdd = g;
                    }

                }
                if (toAdd != null)
                {
                    i++;
                    ans.Add(toAdd);
                }
            }
            return ans;
        }

        private double FutureSingle(List<Gate> R, Dictionary<Gate, double> compHS, double sysRepair)//changes
        {
            double eval = 0;
            double wastedCost = 0;
            double fncost = 0;
            foreach (Gate g in compHS.Keys)
            {
                if (R.Contains(g))
                    wastedCost += (1 - compHS[g]) * g.Cost;
                else
                    fncost += (compHS[g]);//changes
            }
            fncost = fncost * Overhead;
            eval = wastedCost + (fncost * (1 - sysRepair));
            return eval;
        }
        private double FutureBatch(List<Gate> R, Dictionary<Gate, double> compHS, double sysRepair)//changes
        {
            double eval = 0;
            double wastedCost = 0;
            foreach (Gate g in R)
            {
                if (compHS.ContainsKey(g))
                    wastedCost += (1 - compHS[g]) * g.Cost;
                // else
                //    return -1;
            }
            eval = wastedCost + ((1 - sysRepair) * Overhead);
            return eval;
        }
        private double calcSysRepair(List<Gate> R, List<List<Gate>> diagnosis, List<double> diagProb) //changes
        {
            double sysRepair = 0;
            for (int i = 0; i < diagnosis.Count; i++)
            {
                List<Gate> diag = diagnosis[i];
                foreach (Gate g in R)
                {
                    if (diag.Contains(g))
                    {
                        sysRepair += diagProb[i];
                        break;
                    }
                }
            }
            return sysRepair;
        }
        private List<Gate> evalBatchUnion(List<Gate> R, Dictionary<Gate, double> compHS, List<List<Gate>> diagnosis, List<double> diagProb, int k)
        {
            if (k >= K)
                return R;
            List<Gate> ans = null;
            foreach (List<Gate> diag in diagnosis)
            {
                List<Gate> temp = null;
                foreach (Gate g in diag)
                {
                    if (!R.Contains(g))
                    {
                        if (temp == null)
                            temp = new List<Gate>(R);
                        temp.Add(g);
                    }

                }
                if (temp != null)
                {
                    double sysRepair = calcSysRepair(temp, diagnosis, diagProb);
                    double eval = FutureBatch(temp, compHS, sysRepair);
                    if (ans == null || Min == -1 || eval < Min)
                    {
                        Min = eval;
                        ans = temp;
                    }
                    if (k + 1 < K)
                    {
                        ans = evalBatchUnion(temp, compHS, diagnosis, diagProb, k + 1);
                    }
                }
            }
            return ans;
        }
        private List<Gate> evalBatchSubset(List<Gate> R, Dictionary<Gate, double> compHS, List<List<Gate>> diagnosis, List<double> diagProb, List<Gate> compList)
        {
            if (R.Count >= K)
                return R;
            List<Gate> ans = null;
            foreach (Gate g in compList)
            {
                if (!R.Contains(g))
                {
                    List<Gate> temp = new List<Gate>(R);
                    temp.Add(g);
                    double sysRepair = calcSysRepair(temp, diagnosis, diagProb);
                    double eval = FutureBatch(temp, compHS, sysRepair);
                    if (ans == null || Min == -1 || eval < Min)
                    {
                        Min = eval;
                        ans = temp;
                    }
                    if (temp.Count < K)
                    {
                        ans = evalBatchSubset(temp, compHS, diagnosis, diagProb, compList);
                    }
                }
            }
            return ans;
        }
        private List<Gate> evalSingleUnion(List<Gate> R, Dictionary<Gate, double> compHS, List<List<Gate>> diagnosis, List<double> diagProb, int k)
        {
            if (k >= K)
                return R;
            List<Gate> ans = null;
            foreach (List<Gate> diag in diagnosis)
            {
                List<Gate> temp = null;
                foreach (Gate g in diag)
                {
                    if (!R.Contains(g))
                    {
                        if (temp == null)
                            temp = new List<Gate>(R);
                        temp.Add(g);
                    }

                }
                if (temp != null)
                {
                    double sysRepair = calcSysRepair(temp, diagnosis, diagProb);
                    double eval = FutureSingle(temp, compHS, sysRepair);
                    if (ans == null || Min == -1 || eval < Min)
                    {
                        Min = eval;
                        ans = temp;
                    }
                    if (k + 1 < K)
                    {
                        ans = evalSingleUnion(temp, compHS, diagnosis, diagProb, k + 1);
                    }
                }
            }
            return ans;
        }
        private List<Gate> evalSingleSubset(List<Gate> R, Dictionary<Gate, double> compHS, List<Gate> compList, List<List<Gate>> diagnosis, List<double> diagProb)
        {
            if (R.Count >= K)
                return R;
            List<Gate> ans = null;
            foreach (Gate g in compList)
            {
                if (!R.Contains(g))
                {
                    List<Gate> temp = new List<Gate>(R);
                    temp.Add(g);
                    double sysRepair = calcSysRepair(temp, diagnosis, diagProb);
                    double eval = FutureSingle(temp, compHS, sysRepair);
                    if (ans == null || Min == -1 || eval < Min)
                    {
                        Min = eval;
                        ans = temp;
                    }
                    if (temp.Count < K)
                    {
                        ans = evalSingleSubset(temp, compHS, compList, diagnosis, diagProb);
                    }
                }
            }
            return ans;
        }
    }
}

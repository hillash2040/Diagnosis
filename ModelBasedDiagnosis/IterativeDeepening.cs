using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace ModelBasedDiagnosis
{
    class IterativeDeepening:SearchAlgorithm
    {
        private List<Gate> openList;
        private List<Gate> notClosed;
        private List<Gate> notInDiag;
        private Stopwatch stopwatch;
       // public int num;
        //public double dis;
        
        public IterativeDeepening(IFunction function, TimeSpan timeSpan)
            : base(function, timeSpan)
        {
            openList = new List<Gate>();
            notClosed = new List<Gate>();
            stopwatch = new Stopwatch();
        }
        public IterativeDeepening(IFunction function):base(function)
        {
            openList = new List<Gate>();
            notClosed = new List<Gate>();
            stopwatch = new Stopwatch();
        }
        public IterativeDeepening() : this(null) { }
        
        public override List<Diagnosis> FindDiagnoses(Observation observation)
        {
            if (function == null || observation == null || observation.TheModel == null || observation.TheModel.Components == null || observation.TheModel.Components.Count == 0)
                return null; //throw
            this.observation = observation;
            List<Diagnosis> diagnoses = new List<Diagnosis>();
            diagnosesCounter=0;
            hSVector = new List<double>();
            notInDiag = new List<Gate>();
            observation.TheModel.SetValue(observation.InputValues);
           // FindMinCard = false;
            trie = new Trie<string>();
           // stopwatch.Start(); 
            bool toContinue;
            foreach (Gate Component in observation.TheModel.Components)
            {
                if (closed.Contains(Component.Id))
                {
                    if (Component is Cone)
                    {
                        if (((Cone)Component).cone.Components.Count == 0)
                            continue;
                        else
                        {
                            toContinue = true;
                            foreach (Gate g in ((Cone)Component).cone.Components)
                            {
                                if (!closed.Contains(g.Id)) 
                                {
                                    toContinue = false;
                                    break;
                                }
                            }
                            if (toContinue)
                                continue;
                        }
                    }
                    else
                        continue;
                }
                    
                function.Operate(Component);
                if (isDamaged())
                {
                    List<Gate> diag = new List<Gate>();
                    diag.Add(Component);
                    diagnoses.Add(new Diagnosis(diag));
                    trie.Put(Component.Id + "", Component.Id + "");
                   // diagnosesCounter++;
                }
                else
                    notClosed.Add(Component);
                
                Component.SetValue();
            }
            notInDiag.AddRange(notClosed);
            int depth;
            toContinue = true;
            if (CheckHSDistance(5, 0.1, diagnoses, observation.TheModel.Components))
                toContinue = false;
            for (depth = 2; toContinue&&depth <= notClosed.Count; depth++)
            {
                //toContinue = !CheckHSDistance(5, 0.1, diagnoses, observation.TheModel.Components);
                if (Stop())
                    break;
               /* if(diagnoses.Count>0)
                {
                    FindMinCard = true;
                    break;
                }*/
                foreach (Gate g in notClosed)
                {
                    if (Stop()||!toContinue)
                    {
                        break;
                    }
                    openList.Add(g);
                    trie.Matcher.ResetMatch();
                    toContinue = deepCheck(depth,diagnoses,observation.TheModel.Components);
                    openList.Remove(g);
                }
            }
            notClosed.Clear();
            openList.Clear();
            stopwatch.Stop();
            stopwatch.Reset();
            return diagnoses;
        }

        private bool deepCheck(int depth,List<Diagnosis> diagnoses,List<Gate> components)
        {
            bool ans = true;
            int index = notClosed.IndexOf(openList.Last()); //index in notClosed of last Component that added to openlist
            for(int i=index+1; ans==true&&i>=0&&i< notClosed.Count;i++)
            {
                Gate Component = notClosed[i];
                //maybe add check - openlist.last.id < Component
                openList.Add(Component);
                if (depth == 2)
                {
                    if (openList.Count > 2 && isInTrie(openList))
                    {
                        openList.Remove(Component);
                        continue;
                    }
                    string str = "";
                    List<Gate> diag = new List<Gate>();
                    foreach (Gate g in openList)
                    {
                        function.Operate(g);
                        
                        if (g != openList.Last())
                            str += g.Id + " ";
                        else
                            str += g.Id;
                        diag.Add(g);
                    }
                    if (isDamaged())
                    {
                        diagnoses.Add(new Diagnosis(diag));
                        trie.Put(str, str);
                        foreach (Gate g in openList)
                        {
                            if (notInDiag.Contains(g))
                                notInDiag.Remove(g);
                        }
                        //diagnosesCounter++;
                        if(CheckHSDistance(5,0.1,diagnoses,components))
                            ans=false;
                    }
                    foreach (Gate g in openList) 
                    {
                        g.SetValue();
                    }

                }

                else if (!isInTrie(openList))
                {
                    ans = deepCheck(depth-1, diagnoses,components);
                }

                openList.Remove(Component);
            }
            return ans;
        }
        private bool CheckHSDistance(int num, double distance, List<Diagnosis> diagnoses,List<Gate> components)
        {
            if (diagnoses.Count - diagnosesCounter < num)
                return false;
            bool ans = false;
            CalcDiagProb(diagnoses);
            List<double> newHSVector = CalcHelthState(components, diagnoses);
            double dis = CalcHSVectorDistance(newHSVector);
            hSVector = newHSVector;
            diagnosesCounter = diagnoses.Count;
            if (dis <= distance)
                ans = true;
            return ans;          
        }
        public override bool Stop() 
        {
            stopwatch.Stop();
            TimeSpan time = stopwatch.Elapsed;
            stopwatch.Start();
            if (time > x)
                return true;
            else
                return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ModelBasedDiagnosis
{
    class ConesAlgorithm:IDiagnoser
    {
        private SearchAlgorithm searchAlgorithm;
        public ConesAlgorithm(SearchAlgorithm searchAlgo)
        {
            searchAlgorithm = searchAlgo;
        }
        public void addToGoodCompList(int compID)
        {
            searchAlgorithm.addToGoodCompList(compID);
        }
        public void resetGoodCompList()
        {
            searchAlgorithm.resetGoodCompList();
        }
        public List<double> CalcHelthState(List<Gate> components, List<Diagnosis> diagnoses)
        {
            return searchAlgorithm.CalcHelthState(components, diagnoses);
        }
        public void CalcDiagProb(List<Diagnosis> diagnoses)
        {
            searchAlgorithm.CalcDiagProb(diagnoses);
        }
        public List<Diagnosis> FindDiagnoses(Observation observation)
        {
            if (searchAlgorithm == null || observation == null || observation.TheModel == null || observation.TheModel.Components == null || observation.TheModel.Components.Count == 0)
                return null; //throw
            List<Diagnosis> diagnoses = new List<Diagnosis>();
            List<Diagnosis> abstractDiag = new List<Diagnosis>();
            if (observation.TheModel.cones.Count == 0)
                observation.TheModel.createCones();
            List<Cone> cones = observation.TheModel.cones;
            SystemModel toTest = new SystemModel(observation.TheModel.Id, observation.TheModel.Input, observation.TheModel.Output);
            foreach (Cone c in cones)
            {
                toTest.AddComponent(c);
            }
            Observation obs = new Observation(observation.Id, observation.InputValues, observation.OutputValues);
            obs.TheModel = toTest;
            abstractDiag = searchAlgorithm.FindDiagnoses(obs);
            int count = 0;
            Dictionary<int, List<Diagnosis>> coneDiagDic = new Dictionary<int, List<Diagnosis>>();
            List<List<Gate>> tempDiag = new List<List<Gate>>();
            List<List<Gate>> temp = new List<List<Gate>>();
            foreach (Diagnosis diag in abstractDiag)
            {
                List<Gate> openList = diag.diagnosis;
               // observation.TheModel.SetValue(observation.InputValues); 
                observation.SetWiresToCorrectValue();
                if (openList.Count == 0)
                    continue;
                foreach (Cone c in openList)
                {
                    if (coneDiagDic.ContainsKey(c.Id))
                        continue;
                    count++;
                    bool[] obIn = new bool[c.cone.Input.Count];
                    bool[] obOut = new bool[1];
                    for (int i = 0; i < obIn.Length; i++)
                    {
                        obIn[i] = c.cone.Input[i].Value;
                    }
                    obOut[0] = !c.Output.Value;
                    obs = new Observation(observation.Id + count, obIn, obOut);
                    obs.TheModel = c.cone;
                    coneDiagDic.Add(c.Id, searchAlgorithm.FindDiagnoses(obs));
                    c.Output.Value = obOut[0];
                }
                foreach (Cone c in openList) //X
                {
                    if (coneDiagDic[c.Id] == null || coneDiagDic[c.Id].Count == 0)
                        continue;
                    if (tempDiag.Count == 0)
                    {
                        foreach (Diagnosis d in coneDiagDic[c.Id])
                        {
                            tempDiag.Add(new List<Gate>(d.diagnosis));
                        }
                        continue;
                    }
                    if (coneDiagDic[c.Id].Count == 1)
                    {
                        for (int i = 0; i < tempDiag.Count; i++)
                            tempDiag[i].AddRange(new List<Gate>(coneDiagDic[c.Id][0].diagnosis));
                        continue;
                    }
                    temp.AddRange(tempDiag);
                    tempDiag.Clear();
                    foreach (Diagnosis d in coneDiagDic[c.Id])
                    {
                        List<Gate> listD =d.diagnosis;
                        foreach (List<Gate> listTemp in temp)
                        {
                            List<Gate> listTempD = new List<Gate>(listTemp);
                            listTempD.AddRange(new List<Gate>(listD));
                            tempDiag.Add(new List<Gate>(listTempD));
                            listTempD.Clear();
                        }
                    }
                    temp.Clear();
                }
                foreach (List<Gate> list in tempDiag)
                {
                    diagnoses.Add(new Diagnosis(list));
                }
                tempDiag.Clear();
            }
            return diagnoses;
        }
 
        public bool Stop()//no use in this class
        {
            return false;
        }
    }
}

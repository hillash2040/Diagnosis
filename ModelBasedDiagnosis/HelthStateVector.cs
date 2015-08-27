using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    class HelthStateVector
    {
        public List<Gate> Components { get; private set; }
        public List<double> CurrentHelthState { get; private set; }
        public int Count
        {
            get
            {
                if(CurrentHelthState!=null)
                    return CurrentHelthState.Count;
                else
                    return 0;
            }
        }
        public int DiagnosesCounter {get;private set;}
        public HelthStateVector(List<Gate> components)
        {
            if (components != null)
                this.Components = components;
            else
                Components = new List<Gate>();
            CurrentHelthState = new List<double>();
            DiagnosesCounter=0;
        }

        public void CalcHelthState(DiagnosisSet diagnoses)
        {
            if (diagnoses == null || diagnoses.Count==0)
                return;
            List<double> compHS = new List<double>();
            for (int i = 0; i < Components.Count; i++)
            {
                compHS.Add(0);
            }
            foreach (Diagnosis diag in diagnoses.Diagnoses)
            {
                foreach (Gate comp in diag.TheDiagnosis)
                {
                    int i = Components.IndexOf(comp); //check 
                    if (i >= 0 && i < compHS.Count)
                        compHS[i] += (diag.Probability / diagnoses.SetProbability);
                }
            }
            CurrentHelthState = compHS;
            DiagnosesCounter = diagnoses.Count;
        }

        public double GetCompHealthState(Gate component)
        {
            if (component == null)
                return 0;
            int i = Components.IndexOf(component); //check 
            if (i >= 0 && i < Count)
                return CurrentHelthState[i];
            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    class Diagnosis
    {
        public List<Gate> diagnosis { get; private set; }
        public double probability { get; private set; }

        public Diagnosis()
        {
            diagnosis = new List<Gate>();
            probability = 0;
        }
        public Diagnosis(List<Gate> diagnosis)
        {
            if (diagnosis != null)
                this.diagnosis = diagnosis;
            else
                this.diagnosis = new List<Gate>();
            probability = 0;
        }
        public Diagnosis(List<Gate> diagnosis, double probability):this(diagnosis)
        {
            this.probability = probability;
        }
        public void AddCompsToDiagnosis(List<Gate> comps)
        {
            if (comps != null && comps.Count != 0)
            {
                foreach (Gate comp in comps)
                    AddCompToDiagnosis(comp);
            }
        }
        public void AddCompToDiagnosis(Gate comp)
        {
            if (comp != null && !diagnosis.Contains(comp))
                diagnosis.Add(comp);
        }
        public void CalcAndSetProb()
        {

        }
        public void ChangeProbability(double prob)
        {
            if (prob >= 0)
                this.probability = prob;
        }
    }
}

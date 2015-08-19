using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    class DiagnosisSet
    {
        public List<Diagnosis> Diagnoses { get; private set; }
        public double SetProbability { get; set; }
        public int Count
        {
            get
            {
                if (Diagnoses == null)
                    return 0;
                else
                    return Diagnoses.Count;
            }
        }

        public DiagnosisSet()
        {
            Diagnoses = new List<Diagnosis>();
            SetProbability = 0;
        }

        public void AddDiagnosis(Diagnosis diagnosis)
        {
            if (diagnosis != null && diagnosis.TheDiagnosis != null && diagnosis.TheDiagnosis.Count != 0)
            {
                Diagnoses.Add(diagnosis);
                if (diagnosis.Probability == 0)
                    diagnosis.CalcAndSetProb();
                SetProbability += diagnosis.Probability;
            }
        }
        public double Cost()
        {
            if (Count == 0)
                return 0;
            double ans = 0;
            foreach(Diagnosis diag in this.Diagnoses)
            {
                ans += diag.Cost();
            }
            return ans;
        }
        public override bool Equals(object obj)
        {
            if (obj is DiagnosisSet)
            {
                DiagnosisSet diagSet = (DiagnosisSet)obj;
                if (diagSet == null || diagSet.Diagnoses == null)
                    return false;
                if (this.Diagnoses == null || this.Diagnoses.Count != diagSet.Diagnoses.Count)
                    return false;
                foreach (Diagnosis diag in this.Diagnoses)
                {
                    if (!diagSet.Diagnoses.Contains(diag))
                        return false;
                }
                return true;
            }
            else
                return base.Equals(obj);
        }
    }
}

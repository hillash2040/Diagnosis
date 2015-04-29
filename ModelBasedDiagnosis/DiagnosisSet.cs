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

        public DiagnosisSet()
        {
            Diagnoses = new List<Diagnosis>();
            SetProbability = 0;
        }

        public void AddDiagnosis(Diagnosis diagnosis)
        {
            if (diagnosis != null && diagnosis.diagnosis != null && diagnosis.diagnosis.Count != 0)
            {
                Diagnoses.Add(diagnosis);
                if (diagnosis.probability == 0)
                    diagnosis.CalcAndSetProb();
                SetProbability += diagnosis.probability;
            }
        }
    }
}

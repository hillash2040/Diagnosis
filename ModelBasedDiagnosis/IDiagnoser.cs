using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    interface IDiagnoser
    {
        List<Diagnosis> FindDiagnoses(Observation observation);
        List<double> CalcHelthState(List<Gate> components, List<Diagnosis> diagnoses);
        void CalcDiagProb(List<Diagnosis> diagnoses);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    class SystemState
    {
        private DiagnosisSet m_diagnoses;
        public DiagnosisSet Diagnoses
        {
            get
            {
                return m_diagnoses;
            }
            set
            {
                m_diagnoses = value;
                if (HelthState != null)
                    HelthState.CalcHelthState(m_diagnoses);
            }
        }
        public HelthStateVector HelthState;

    }
}

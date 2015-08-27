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
                if (HelthState != null && HelthState.Count>0)
                    HelthState.CalcHelthState(m_diagnoses);
            }
        }

        public HelthStateVector HelthState { get; private set; }

        public SystemState(List<Gate> components)
        {
            if (components != null && components.Count != 0)
                HelthState = new HelthStateVector(components);
        }
        
        public SystemState GetNextState(List<Gate> action)
        {
            DiagnosisSet nextDiagSet = computeNextState(action);
            if (nextDiagSet == null)
                return this; // return null?
            SystemState nextState=null;
            if (this.HelthState == null)
                nextState = new SystemState(null);
            else
                nextState = new SystemState(HelthState.Components);
            nextState.Diagnoses = nextDiagSet;
            return nextState;
        }
        public void SetNextState(List<Gate> action)
        {
            DiagnosisSet nextDiagSet = computeNextState(action);
            if (nextDiagSet == null)
                return;
            Diagnoses = nextDiagSet;
        }
        private DiagnosisSet computeNextState(List<Gate> action)
        {
            if (m_diagnoses == null || m_diagnoses.Count == 0 || action == null || action.Count == 0)
                return null;
            DiagnosisSet ans = new DiagnosisSet();
            foreach (Diagnosis diag in m_diagnoses.Diagnoses)
            {
                Diagnosis toAdd = new Diagnosis();
                foreach (Gate g in diag.TheDiagnosis)
                {
                    if (!action.Contains(g))
                    {
                        toAdd.AddCompToDiagnosis(g);
                    }
                }
                if (toAdd.TheDiagnosis.Count == diag.TheDiagnosis.Count) //to save space and time
                    toAdd = diag;
                if (toAdd.TheDiagnosis.Count != 0)
                    ans.AddDiagnosis(toAdd);
            }
            return ans;
        }

        public override bool Equals(object obj) //comparing only the diagnosis set
        {
            if (obj is SystemState)
            {
                SystemState state = (SystemState)obj;
                if (state == null || state.Diagnoses == null || this.Diagnoses == null)
                    return false;
                return this.m_diagnoses.Equals(state.m_diagnoses);
            }
            else
                return base.Equals(obj);
        }

        public double SystemRepair(List<Gate> repairAction) 
        {
            double sysRepair = 0;
            if (m_diagnoses == null || repairAction == null || repairAction.Count == 0)
                return sysRepair;
            if (m_diagnoses.Count == 0)
                return 1; //system is repaired
            foreach (Diagnosis diag in m_diagnoses.Diagnoses)
            {
                if (diag.TheDiagnosis == null || diag.TheDiagnosis.Count == 0 || diag.TheDiagnosis.Count > repairAction.Count)
                    continue;
                double p = diag.Probability;
                foreach (Gate g in diag.TheDiagnosis)
                {
                    if (!repairAction.Contains(g))
                    {
                        p = 0;
                        break;
                    }

                }
                sysRepair += p;
            }
            return sysRepair;
        }

    }
}

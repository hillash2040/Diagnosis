﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    abstract class BatchPlanner
    {
        //public List<Gate> Plan(DiagnosisSet diagnoses);
        public abstract List<Gate> Plan(SystemState state);
    }
}

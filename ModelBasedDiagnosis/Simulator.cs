using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace ModelBasedDiagnosis
{
    class Simulator
    {
        private IDiagnoser Diagnoser;
        //private Planner planner;
        private ModelObservationCreator MOCreator;
        private IFunction gateFunc;

        public Simulator()
        {
            MOCreator = new ModelObservationCreator();
        }
        public Simulator(IDiagnoser Algorithm)
        {
            Diagnoser = Algorithm;
            if (Diagnoser != null && Diagnoser is DiagnosesSearcher)
            {
                this.gateFunc = ((DiagnosesSearcher)Diagnoser).function;
            }
            else
                this.gateFunc = new FlipFunction();
            MOCreator = new ModelObservationCreator();
        }
        private void createNewDiagnoser()
        {
            FlipFunction flip = new FlipFunction();
            TimeSpan x = new TimeSpan(0, 1, 0);
            IterativeDeepening salgo = new IterativeDeepening(flip, x);
            ConesAlgorithm algo = new ConesAlgorithm(salgo);
            this.Diagnoser = algo;
        }
        public void temp(string fileModel, string fileObs, string fileReal)
        {
            createNewDiagnoser();
            RepairActionSearcher ras = new PowersetBasedSearcher();
            BatchCostEstimator bce = new PessimisticEstimator();
            MDPPlanner planner = new MDPPlanner(ras, bce, 2);
            List<Observation> observationsList = MOCreator.ReadObsModelFiles(fileModel, fileObs);
            Dictionary<int, List<int>> obsReal = MOCreator.ReadRealObsFiles(fileReal);
            if (observationsList == null || observationsList.Count == 0 || obsReal == null || obsReal.Count == 0)
                return;
            Observation obs = observationsList[0];
            List<int> realComp = new List<int>(obsReal[obs.Id]);
            DiagnosisSet diagnoses = Diagnoser.FindDiagnoses(obs);
            SystemState state = new SystemState(obs.TheModel.Components);
            state.Diagnoses = diagnoses;
            List<Gate> repairAction = planner.Plan(state);

        }
        public void HS(string fileName1, string fileName2)
        {
            List<Observation> observationsList = MOCreator.ReadObsModelFiles(fileName1, fileName2);
            if (observationsList == null || observationsList.Count == 0)
                return;
            FlipFunction flip = new FlipFunction();
            TimeSpan x = new TimeSpan(0, 5, 0);
            IterativeDeepening salgo = new IterativeDeepening(flip, x);
            ConesAlgorithm algo = new ConesAlgorithm(salgo);
            Stopwatch stopwtch = new Stopwatch();
            CSVExport myExport = new CSVExport();
            salgo.agenda = DiagnosesSearcher.Agenda.helthState;
            foreach (Observation obs in observationsList)
            {
                stopwtch.Start();
                DiagnosisSet diagnoses = algo.FindDiagnoses(obs);
                stopwtch.Stop();
                if (diagnoses != null)
                {
                    myExport.AddRow();
                    myExport["System"] = obs.TheModel.Id;
                    myExport["Observation"] = obs.Id;
                    myExport["# diagnoses"] = diagnoses.Count;
                    myExport["Runtime"] = stopwtch.Elapsed;
                }
                stopwtch.Reset();
            }
            myExport.ExportToFile(observationsList.First().TheModel.Id + "HS.csv");

        }
        public void CheckMinCard(string fileName1, string fileName2)
        {
            List<Observation> observationsList = MOCreator.ReadObsModelFiles(fileName1, fileName2);
            if (observationsList == null || observationsList.Count == 0)
                return;
            FlipFunction flip = new FlipFunction();
            TimeSpan x = new TimeSpan(0, 5, 0);
            IterativeDeepening salgo = new IterativeDeepening(flip,x);
            ConesAlgorithm algo = new ConesAlgorithm(salgo);
            CSVExport myExport = new CSVExport();
            foreach (Observation obs in observationsList)
            {
                algo.CheckMinCard(obs, myExport);
            }
            myExport.ExportToFile(observationsList.First().TheModel.Id+"MinCard.csv");
        }

        public void CheckPropogation(string fileName1, string fileName2)
        {
            List<Observation> observationsList = MOCreator.ReadObsModelFiles(fileName1, fileName2);
            if (observationsList == null || observationsList.Count == 0)
                return;
            Stopwatch stopwtch = new Stopwatch();
            Observation obs = observationsList[1];
            stopwtch.Start();
            obs.TheModel.SetValue(obs.InputValues);
            stopwtch.Stop();
            TimeSpan time1 = stopwtch.Elapsed;
            stopwtch.Restart();
            obs.SetWiresToCorrectValue();
            stopwtch.Stop();
            TimeSpan time2 = stopwtch.Elapsed;
            StreamWriter sw = new StreamWriter(obs.TheModel.Id+"proptime.txt");
            sw.Write(time1 + " " + time2);
            sw.Close();
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBasedDiagnosis
{
    class MDPPlanner : EvalFunctionBatchPlanner
    {
        private HashSet<SystemState> visitedStates; // DiagnosisSet / MDPState?
        public MDPPlanner(RepairActionSearcher repairActionSearcher, BatchCostEstimator costeEstimator)
            : base(repairActionSearcher, costeEstimator)
        {
            visitedStates = new HashSet<SystemState>();
        }
        public MDPPlanner(RepairActionSearcher repairActionSearcher, BatchCostEstimator costeEstimator, int k)
            : base(repairActionSearcher, costeEstimator,k)
        {
            visitedStates = new HashSet<SystemState>();
        }

        public void ResetVisitedStates()
        {
            if (visitedStates.Count > 0)
                visitedStates.Clear();
        }
        public override List<Gate> Plan(SystemState state)
        {
            if (state == null || state.Diagnoses == null || state.Diagnoses.Count == 0)
                return null;
            MDPState mdpState = new MDPState(state);
            double utility = CalcUtility(mdpState, 3); //k-parameter?
            if(mdpState.Actions==null||mdpState.Actions.Count==0)
                return null;
            return mdpState.Actions[0];
        }
        public double CalcUtility(MDPState state, int k)
        {
            double ans = 0;
            //double ans= double.MinValue;
            if (state == null || state.IsEmptyState())
                return ans;
            if (k == 0)
            {
                // heuristic  
                ans = -costeEstimator.StateCost(state.State);//(-)!
                return ans;
            }
            List<List<Gate>> actions = repairActionSearcher.ComputePossibleAcions(state.State,K);
            if (actions == null || actions.Count == 0)
                return ans;//
            ans =double.MinValue;
            List<Gate> bestAction = null;
            foreach (List<Gate> a in actions)
            {
                if (a == null || a.Count == 0)
                    continue;
                double U = -costeEstimator.ComputeRepairCost(a); //(-)!
                MDPState nextState = ComputeNextState(state, a);
                if (nextState != null && !nextState.IsEmptyState()) 
                {
                    U += TransitionFunc(state, a) * CalcUtility(nextState, k-1); 
                }
                if (U > ans || bestAction==null)
                {
                    ans = U;
                    bestAction = a;
                }   
            }
            if (bestAction != null)
                state.Actions.Add(bestAction);
            return ans;
        }
        public MDPState ComputeNextState(MDPState state, List<Gate> action)
        {
            SystemState nextSysState = state.State.GetNextState(action);
            if (visitedStates.Contains(nextSysState))//
                return null;//
            MDPState nextState = new MDPState();
            nextState.State = nextSysState;
            nextState.Actions.AddRange(state.Actions);
            nextState.Actions.Add(action);
            return nextState;
        }
        public double TransitionFunc(MDPState state, List<Gate> action) //s'=system is not fixed
        {
            return (1 - state.State.SystemRepair(action));
        }
       /* public List<List<Gate>> ComputePossibleActions(MDPState state)
        {
            return null;
        }*/

    }
}

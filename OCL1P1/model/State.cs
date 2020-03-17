using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.model
{
    class State
    {
        private string stateName;
        private bool isEnd;
        private List<Transition> transitions;
        private List<State> epsilonTransitions;

        public State(string stateName, bool isEnd, List<Transition> transition, List<State> epsilonTransitions)
        {
            this.stateName = stateName;
            this.isEnd = isEnd;
            this.transitions = transition;
            this.epsilonTransitions = epsilonTransitions;
        }

        public string StateName { get => stateName; set => stateName = value; }
        public bool IsEnd { get => isEnd; set => isEnd = value; }
        internal List<Transition> Transitions { get => transitions; set => transitions = value; }
        internal List<State> EpsilonTransitions { get => epsilonTransitions; set => epsilonTransitions = value; }
    }
}

using OCL1P1.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.controller
{
    class SubsetConstruction
    {
        private List<Transition> epsilonTransitions;
        private List<Transition> symbolTransitions;
        private List<Token> terminals;
        private List<State> states;
        private int indexState;

        private List<Transition> transitionsNFA;
        private List<State> initialState;

        internal List<Transition> Transitions { get; set; }
        internal List<Subset> Subsets { get; set; }

        public SubsetConstruction(List<Transition> tNFA, List<State> sNFA, List<Token> terminalsNFA)
        {
            epsilonTransitions = new List<Transition>();
            symbolTransitions = new List<Transition>();
            terminals = new List<Token>();
            states = new List<State>();
            Subsets = new List<Subset>();
            Transitions = new List<Transition>();

            transitionsNFA = new List<Transition>();
            initialState = new List<State>();

            transitionsNFA.AddRange(tNFA);
            initialState.Add(sNFA[0]);
            terminals.AddRange(terminalsNFA);
            states.AddRange(sNFA);

            indexState = 0;

            NFATable();
        }

        private void NFATable()
        {
            foreach (Transition transition in transitionsNFA)
            {
                if (transition.Token.TypeToken == Token.Type.EPSILON && transition.From != null)
                {
                    epsilonTransitions.Add(transition);
                }
                else if (transition.Token.TypeToken != Token.Type.EPSILON)
                {
                    symbolTransitions.Add(transition);
                }
            }
        }

        public void Construction()
        {
            Cerradura(initialState);
        }

        private Subset Cerradura(List<State> statesList)
        {
            if (statesList.Count() > 0)
            {
                List<State> cStates = CSets(statesList);
                Subset subset = CreateSubset(cStates);

                foreach (Token item in terminals)
                {
                    List<State> moveStates = Move(cStates, item);
                    Subset toSubset = Cerradura(moveStates);

                    if (subset != null && toSubset != null)
                    {
                        State from = new State(subset.Name);
                        State to = new State(toSubset.Name);

                        if (toSubset.States.Find(x => x.StateName == states.Last().StateName) != null)
                        {
                            to.IsEnd = true;
                        }

                        Transitions.Add(new Transition(from, item, to));
                    }
                }
                return subset;
            }
            return null;
        }

        private List<State> CSets(List<State> statesList)
        {
            List<State> cStates = new List<State>();

            foreach (State state in statesList)
            {
                cStates.Add(state);
                foreach (Transition epsilonT in epsilonTransitions)
                {
                    if (epsilonT.From.StateName == state.StateName)
                    {
                        cStates.AddRange(CSets(new List<State>() { epsilonT.To }));
                    }
                }
            }

            return cStates;
        }

        private List<State> Move(List<State> statesList, Token token)
        {
            List<State> moveStates = new List<State>();

            foreach (State state in statesList)
            {
                foreach (Transition transition in symbolTransitions)
                {
                    if (state.StateName == transition.From.StateName
                        && transition.Token.Value == token.Value)
                    {
                        moveStates.Add(transition.To);
                    }
                }
            }

            return moveStates;
        }

        private Subset CreateSubset(List<State> statesList)
        {
            Subset subset = Subsets.Find(r => r.States == statesList.OrderBy(or => or.StateName).ToList());

            if (subset == null)
            {
                subset = new Subset("S" + indexState, statesList.OrderBy(or => or.StateName).ToList());
                Subsets.Add(subset);
                indexState++;
            }

            return subset;
        }
    }
}

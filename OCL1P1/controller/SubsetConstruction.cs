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
        private int indexState;
        private bool findState;

        private List<Transition> transitionsNFA;
        private List<State> statesNFA;
        private List<State> initialState;

        internal List<Transition> Transitions { get; set; }
        internal List<Subset> Subsets { get; set; }
        internal List<State> States { get; set; }

        public SubsetConstruction(List<Transition> tNFA, List<State> sNFA, List<Token> terminalsNFA)
        {
            epsilonTransitions = new List<Transition>();
            symbolTransitions = new List<Transition>();
            terminals = new List<Token>();
            Subsets = new List<Subset>();
            Transitions = new List<Transition>();
            States = new List<State>();

            transitionsNFA = new List<Transition>();
            statesNFA = new List<State>();
            initialState = new List<State>();

            transitionsNFA.AddRange(tNFA);
            initialState.Add(sNFA[0]);
            terminals.AddRange(terminalsNFA);
            statesNFA.AddRange(sNFA);

            indexState = 0;
            findState = false;

            NFATable();
        }

        private void NFATable()
        {
            foreach (Transition transition in transitionsNFA)
            {
                if ((transition.Token == null
                    || transition.Token.TypeToken == Token.Type.EPSILON)
                    && transition.From != null)
                {
                    epsilonTransitions.Add(transition);
                }
                else if (transition.Token != null && transition.From != null && transition.Token.TypeToken != Token.Type.EPSILON)
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

                if (subset != null && findState == false)
                {
                    foreach (Token item in terminals)
                    {
                        List<State> moveStates = Move(cStates, item);
                        Subset toSubset = Cerradura(moveStates);

                        State from = new State(subset.Name);
                        if (subset.States.Find(x => x.StateName == statesNFA.Last().StateName) != null)
                        {
                            from.IsEnd = true;
                        }
                        if (States.Find(x => x.StateName == from.StateName) == null)
                        {
                            States.Add(from);
                        }

                        if (toSubset != null)
                        {
                            State to = new State(toSubset.Name);
                            if (toSubset.States.Find(x => x.StateName == statesNFA.Last().StateName) != null)
                            {
                                to.IsEnd = true;
                            }
                            if (States.Find(x => x.StateName == to.StateName) == null)
                            {
                                States.Add(to);
                            }

                            Transitions.Add(new Transition(from, item, to));
                        }
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
            foreach (Subset item in Subsets)
            {
                if (!item.States.Except(statesList).Any())
                {
                    findState = true;
                    return item;
                }
            }

            Subset subset = new Subset("S" + indexState, statesList.OrderBy(or => or.StateName).ToList());
            Subsets.Add(subset);
            indexState++;
            findState = false;
            return subset;
        }

        public string[,] StatesMatrix()
        {
            List<Token> terminalsM = new List<Token>();
            int indexState = 0;
            int indexTerminal = 0;

            foreach (Token item in terminals.OrderBy(x => x.Value).ToList())
            {
                if (item.TypeToken != Token.Type.EPSILON)
                {
                    terminalsM.Add(item);
                }
            }
            string[,] statesM = new string[States.Count() + 1, terminalsM.Count() + 1];

            for (int i = 0; i < terminalsM.Count(); i++)
            {
                statesM[0, i + 1] = terminalsM[i].Value;
            }

            States = States.OrderBy(x => x.StateName).ToList();
            for (int i = 0; i < States.Count(); i++)
            {
                statesM[i + 1, 0] = States[i].StateName;
            }

            foreach (Transition item in Transitions)
            {
                indexState = States.FindIndex(a => a.StateName == item.From.StateName) + 1;
                indexTerminal = terminalsM.FindIndex(a => a.Value == item.Token.Value) + 1;
                statesM[indexState, indexTerminal] = item.To.StateName;
            }

            return statesM;
        }
    }
}

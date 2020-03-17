using OCL1P1.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.controller
{
    class NFA
    {
        Symbol symbol;
        List<Token> tokens;
        int index;

        public NFA(Symbol symbol)
        {
            this.symbol = symbol;
            tokens = symbol.Value;
            index = 0;
        }

        public Transition Thompson()
        {
            Token token = tokens.ElementAt(index);
            State rootState = new State("S" + index, false, null, null);
            Transition rootTransition = new Transition(rootState, null);

            index++;
            switch (token.TypeToken)
            {
                case Token.Type.CONCATENATION_SIGN:
                    Transition n1C = Thompson();
                    Transition n2C = Thompson();

                    rootTransition.State.Transitions.Add(n1C);
                    n1C.State.Transitions.Add(n2C);
                    break;
                case Token.Type.DISJUNCTION_SIGN:
                    State n1D = new State("S" + index, false, null, null);
                    rootTransition.State.EpsilonTransitions.Add(n1D);
                    Transition n2D = Thompson();
                    n1D.Transitions.Add(n2D);

                    State n3D = new State("S" + index, false, null, null);
                    rootTransition.State.EpsilonTransitions.Add(n3D);
                    Transition n4D = Thompson();
                    n3D.Transitions.Add(n4D);

                    State endStateD = new State("S" + index, false, null, null);
                    n2D.State.EpsilonTransitions.Add(endStateD);
                    n4D.State.EpsilonTransitions.Add(endStateD);
                    break;
                case Token.Type.QUESTION_MARK_SIGN:
                    break;
                case Token.Type.ASTERISK_SIGN:
                    State n1A = new State("S" + index, false, null, null);
                    rootTransition.State.EpsilonTransitions.Add(n1A);
                    Transition n2A = Thompson();
                    n1A.Transitions.Add(n2A);
                    n2A.State.EpsilonTransitions.Add(n1A);
                    State endStateA = new State("S" + index, false, null, null);
                    n2A.State.EpsilonTransitions.Add(endStateA);
                    rootTransition.State.EpsilonTransitions.Add(endStateA);
                    break;
                case Token.Type.PLUS_SIGN:
                    break;
                case Token.Type.ID:
                case Token.Type.NUMBER:
                case Token.Type.STR:
                case Token.Type.SYMBOL:
                    State state = new State("S" + index, false, null, null);
                    return new Transition(state, token);
            }
            return rootTransition;
        }
    }
}

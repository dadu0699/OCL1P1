using OCL1P1.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.controller
{
    class Thompson
    {
        private Symbol symbol;
        private List<Token> tokens;
        private List<Transition> transitions;
        private int indexToken;
        private int indexState;

        public Thompson(Symbol symbol)
        {
            tokens = new List<Token>();
            transitions = new List<Transition>();

            this.symbol = symbol;
            tokens.AddRange(symbol.Value);
            indexToken = 0;
            indexState = 0;
        }

        internal List<Transition> Transitions { get => transitions; set => transitions = value; }

        public NFA Construction()
        {
            Token token = tokens.ElementAt(indexToken);
            State rootState = new State(indexState.ToString());
            Transition rootTransition = new Transition(null, null, rootState);
            NFA rootNFA = new NFA(rootTransition, rootTransition);
            transitions.Add(rootTransition);

            switch (token.TypeToken)
            {
                case Token.Type.CONCATENATION_SIGN:
                    indexToken++;
                    indexState++;
                    NFA n1C = Construction();

                    indexToken++;
                    indexState++;
                    NFA n2C = Construction();

                    n1C.Initial.From = rootNFA.Acceptance.To;
                    n2C.Initial.From = n1C.Acceptance.To;

                    rootTransition.From = rootNFA.Initial.From;
                    rootTransition.To = n1C.Initial.From;
                    rootNFA.Acceptance = n2C.Acceptance;
                    break;
                case Token.Type.DISJUNCTION_SIGN:
                    indexState++;
                    State s1D = new State(indexState.ToString());
                    Transition t1D = new Transition(rootState, null, s1D);
                    transitions.Add(t1D);

                    indexToken++;
                    indexState++;
                    NFA n2D = Construction();
                    n2D.Initial.From = t1D.To;

                    indexState++;
                    State s3D = new State(indexState.ToString());
                    Transition t3D = new Transition(rootState, null, s3D);
                    transitions.Add(t3D);

                    indexToken++;
                    indexState++;
                    NFA n4D = Construction();
                    n4D.Initial.From = t3D.To;

                    indexState++;
                    State s5D = new State(indexState.ToString());
                    Transition t25D = new Transition(n2D.Acceptance.To, null, s5D);
                    transitions.Add(t25D);
                    Transition t45D = new Transition(n4D.Acceptance.To, null, s5D);
                    transitions.Add(t45D);

                    rootNFA.Initial.To = rootState;
                    rootNFA.Acceptance = new Transition(null, null, t45D.To);
                    break;
                case Token.Type.QUESTION_MARK_SIGN:
                    break;
                case Token.Type.ASTERISK_SIGN:
                    break;
                case Token.Type.PLUS_SIGN:
                    break;
                case Token.Type.ID:
                case Token.Type.NUMBER:
                case Token.Type.STR:
                case Token.Type.SYMBOL:
                    rootTransition.Token = token;
                    Console.WriteLine(" -> " + rootTransition.Token.Value + " -> " + rootTransition.To.StateName);
                    break;
            }
            return rootNFA;
        }
    }
}

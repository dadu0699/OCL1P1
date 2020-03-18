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
        private int index;

        public Thompson(Symbol symbol)
        {
            tokens = new List<Token>();
            transitions = new List<Transition>();

            this.symbol = symbol;
            tokens.AddRange(symbol.Value);
            index = 0;
        }

        internal List<Transition> Transitions { get => transitions; set => transitions = value; }

        public NFA Construction()
        {
            Token token = tokens.ElementAt(index);
            State rootState = new State(index.ToString());
            Transition rootTransition = new Transition(null, null, rootState);
            NFA rootNFA = new NFA(rootTransition, rootTransition);
            transitions.Add(rootTransition);

            switch (token.TypeToken)
            {
                case Token.Type.CONCATENATION_SIGN:
                    index++;
                    NFA n1C = Construction();

                    index++;
                    NFA n2C = Construction();

                    n1C.Initial.From = rootNFA.Acceptance.To;
                    n2C.Initial.From = n1C.Acceptance.To;

                    rootTransition.From = rootNFA.Initial.From;
                    rootTransition.To = n1C.Initial.From;
                    rootNFA.Acceptance = n2C.Acceptance;
                    break;
                case Token.Type.DISJUNCTION_SIGN:
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

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
        private int indexToken;
        private int indexState;
        private Token epsilon;

        internal List<Transition> Transitions { get; set; }
        internal List<State> States { get; set; }

        public Thompson(Symbol symbol)
        {
            tokens = new List<Token>();
            Transitions = new List<Transition>();
            States = new List<State>();

            this.symbol = symbol;
            tokens.AddRange(symbol.Value);

            indexToken = 0;
            indexState = 0;
            epsilon = new Token(0, 0, 0, Token.Type.EPSILON, "&epsilon;");

            FactorizedEXP();
        }

        public NFA Construction()
        {
            Token token = tokens.ElementAt(indexToken);
            State rootState = new State(indexState.ToString());
            States.Add(rootState);
            Transition rootTransition = new Transition(null, epsilon, rootState);
            NFA rootNFA = new NFA(rootTransition, rootTransition);
            Transitions.Add(rootTransition);

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
                    States.Add(s1D);
                    Transition t1D = new Transition(rootState, epsilon, s1D);
                    Transitions.Add(t1D);

                    indexToken++;
                    indexState++;
                    NFA n2D = Construction();
                    n2D.Initial.From = t1D.To;

                    indexState++;
                    State s3D = new State(indexState.ToString());
                    States.Add(s3D);
                    Transition t3D = new Transition(rootState, epsilon, s3D);
                    Transitions.Add(t3D);

                    indexToken++;
                    indexState++;
                    NFA n4D = Construction();
                    n4D.Initial.From = t3D.To;

                    indexState++;
                    State s5D = new State(indexState.ToString());
                    States.Add(s5D);
                    Transition t25D = new Transition(n2D.Acceptance.To, epsilon, s5D);
                    Transitions.Add(t25D);
                    Transition t45D = new Transition(n4D.Acceptance.To, epsilon, s5D);
                    Transitions.Add(t45D);

                    rootNFA.Initial.To = rootState;
                    rootNFA.Acceptance = new Transition(null, epsilon, t45D.To);
                    break;
                case Token.Type.ASTERISK_SIGN:
                    indexState++;
                    State s1A = new State(indexState.ToString());
                    States.Add(s1A);
                    Transition t1A = new Transition(rootState, epsilon, s1A);
                    Transitions.Add(t1A);

                    indexToken++;
                    indexState++;
                    NFA n2A = Construction();
                    n2A.Initial.From = t1A.To;
                    Transition t21A = new Transition(n2A.Acceptance.To, epsilon, s1A);
                    Transitions.Add(t21A);

                    indexState++;
                    State s3A = new State(indexState.ToString());
                    States.Add(s3A);
                    Transition t23A = new Transition(n2A.Acceptance.To, epsilon, s3A);
                    Transitions.Add(t23A);


                    Transition t03A = new Transition(rootState, epsilon, s3A);
                    Transitions.Add(t03A);

                    rootNFA.Initial.To = rootState;
                    rootNFA.Acceptance = new Transition(null, epsilon, t03A.To);
                    break;
                case Token.Type.ID:
                case Token.Type.NUMBER:
                case Token.Type.STR:
                case Token.Type.SYMBOL:
                case Token.Type.EPSILON:
                    rootTransition.Token = token;
                    Console.WriteLine(" -> " + rootTransition.Token.Value + " -> " + rootTransition.To.StateName);
                    break;
            }
            return rootNFA;
        }

        private void FactorizedEXP()
        {
            int index = 0;

            for (int i = 0; i < tokens.Count() - 1; i++)
            {
                if (tokens[i].TypeToken == Token.Type.QUESTION_MARK_SIGN)
                {
                    index++;
                    tokens[i] = new Token(0, 0, 0, Token.Type.DISJUNCTION_SIGN, "|");
                    AddEpsilon(i + 1);
                }
                else if (tokens[i].TypeToken == Token.Type.PLUS_SIGN)
                {
                    index++;
                    tokens[i] = new Token(0, 0, 0, Token.Type.CONCATENATION_SIGN, ".");
                    AddKleene(i + 1);
                }
            }
        }

        private void AddEpsilon(int position)
        {
            int index = 1;
            for (int i = position; i < tokens.Count(); i++)
            {
                if (tokens[i].TypeToken == Token.Type.CONCATENATION_SIGN
                        || tokens[i].TypeToken == Token.Type.DISJUNCTION_SIGN)
                {
                    index++;
                }
                else if (tokens[i].TypeToken == Token.Type.ID
                    || tokens[i].TypeToken == Token.Type.NUMBER
                    || tokens[i].TypeToken == Token.Type.SYMBOL
                    || tokens[i].TypeToken == Token.Type.STR
                    || tokens[i].TypeToken == Token.Type.EPSILON)
                {
                    index--;
                }

                if (index == 0)
                {
                    i++;
                    if (i > tokens.Count() - 1)
                    {
                        tokens.Add(epsilon);
                    }
                    else
                    {
                        tokens.Insert(i, epsilon);
                    }
                    break;
                }
            }
        }

        private void AddKleene(int position)
        {
            List<Token> auxTokens = new List<Token>();
            int index = 1;
            for (int i = position; i < tokens.Count(); i++)
            {
                if (tokens[i].TypeToken == Token.Type.CONCATENATION_SIGN
                        || tokens[i].TypeToken == Token.Type.DISJUNCTION_SIGN)
                {
                    index++;
                }
                else if (tokens[i].TypeToken == Token.Type.ID
                    || tokens[i].TypeToken == Token.Type.NUMBER
                    || tokens[i].TypeToken == Token.Type.SYMBOL
                    || tokens[i].TypeToken == Token.Type.STR
                    || tokens[i].TypeToken == Token.Type.EPSILON)
                {
                    index--;
                }

                auxTokens.Add(tokens[i]);

                if (index == 0)
                {
                    i++;
                    if (i > tokens.Count() - 1)
                    {
                        tokens.Add(new Token(0, 0, 0, Token.Type.ASTERISK_SIGN, "*"));
                        tokens.AddRange(auxTokens);
                    }
                    else
                    {
                        auxTokens.Reverse();
                        foreach (Token item in auxTokens)
                        {
                            tokens.Insert(i, item);
                        }
                        tokens.Insert(i, new Token(0, 0, 0, Token.Type.ASTERISK_SIGN, "*"));
                    }
                    break;
                }
            }
        }

        public void RemoveUnnecessaryT()
        {
            List<Transition> auxTransitions = new List<Transition>();
            foreach (Transition transition in Transitions)
            {
                if (transition.Token.TypeToken == Token.Type.EPSILON 
                    && transition.From != null)
                {
                    auxTransitions.Add(transition);
                }
            }

            var numberOfStatesTDuplicates = Transitions.GroupBy(u => u.To)
                .Select(x => new {Count = x.Count(), State = x.Key}).ToList();
            foreach (var item in numberOfStatesTDuplicates)
            {
                if (item.Count > 1)
                {
                    auxTransitions = auxTransitions.Where(x => x.To.StateName != item.State.StateName).ToList();
                    auxTransitions = auxTransitions.Where(x => x.From.StateName != item.State.StateName).ToList();
                }
            }

            var numberOfStatesFDuplicates = Transitions.GroupBy(u => u.From)
                .Select(x => new { Count = x.Count(), State = x.Key }).ToList();
            foreach (var item in numberOfStatesFDuplicates)
            {
                if (item.Count > 1)
                {
                    auxTransitions = auxTransitions.Where(x => x.From.StateName != item.State.StateName).ToList();
                    // auxTransitions = auxTransitions.Where(x => x.To.StateName != item.State.StateName).ToList();
                }
            }



            foreach (var item in auxTransitions)
            {
                Console.WriteLine(item.From.StateName + " -> " + item.Token.Value + " -> " + item.To.StateName);
            }

            Transition auxFrom;
            Transition auxTo;
            foreach (Transition item in auxTransitions)
            {
                auxFrom = null;
                auxTo = null;

                foreach (Transition transition in Transitions)
                {
                    if (item.From == transition.From)
                    {
                        auxFrom = transition;
                    }
                }

                foreach (Transition transition in Transitions)
                {
                    if (item.From == transition.To)
                    {
                        auxTo = transition;
                    }
                }

                if (auxFrom != null && auxTo != null)
                {
                    auxTo.To = auxFrom.To;
                }
            }

            foreach (Transition item in auxTransitions)
            {
                States.Remove(item.From);
                Transitions.Remove(item);
            }

            for (int i = 0; i < States.Count(); i++)
            {
                States[i].StateName = i.ToString();
            }
        }
    }
}

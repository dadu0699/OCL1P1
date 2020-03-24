using OCL1P1.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.controller
{
    class LexemeEvaluation
    {
        private List<char> charsList;
        private List<Transition> transitions;
        private List<SetC> sets;
        private int indexChar;

        public LexemeEvaluation(string lexeme, List<Transition> transitionsAFD, List<SetC> setC)
        {
            charsList = new List<char>();
            transitions = new List<Transition>();
            sets = new List<SetC>();


            charsList.AddRange(lexeme.ToCharArray());
            transitions.AddRange(transitionsAFD);
            sets.AddRange(setC);
            indexChar = 0;
        }

        public void Initialize()
        {
            State state = Evaluation((transitions.OrderBy(x => x.From.StateName)).ToList()[0].From);
            Console.WriteLine();
            if (state != null && state.IsEnd)
            {
                Console.Write("CADENA ACEPTADA: \"");
                for (int i = 0; i < charsList.Count(); i++)
                {
                    Console.Write(charsList[i]);
                }
                Console.WriteLine("\"");
            }
            else
            {
                Console.Write("LA CADENA: \"");
                for (int i = 0; i < charsList.Count(); i++)
                {
                    Console.Write(charsList[i]);
                }
                Console.Write("\" HA DEJADO DE SER ACEPTADA DESDE: \"");
                for (int i = 0; i < indexChar; i++)
                {
                    Console.Write(charsList[i]);
                }
                Console.Write("\" QUEDANDOSE EN EL ESTADO ");
                if (state != null)
                {
                    Console.Write(state.StateName);
                }
                Console.WriteLine();
            }
        }

        private State Evaluation(State state)
        {
            if (state != null && indexChar < charsList.Count())
            {
                List<Transition> statetransitions = transitions.FindAll(s => s.From.StateName == state.StateName);
                char character = charsList[indexChar];
                State toState = null;

                if (char.IsWhiteSpace(character))
                {
                    if (character.CompareTo('\n') == 0 && character.CompareTo('\t') != 0)
                    {
                        indexChar++;
                        character = charsList[indexChar];
                    }
                }

                foreach (Transition transition in statetransitions)
                {
                    bool exitLoop = false;
                    string tokenValue = transition.Token.Value;
                    StringBuilder entry = new StringBuilder();

                    switch (transition.Token.TypeToken)
                    {
                        case Token.Type.LINE_BREAK:
                            if (character.CompareTo('\n') == 0)
                            {
                                toState = transition.To;
                                exitLoop = true;
                                break;
                            }
                            break;
                        case Token.Type.SINGLE_QUOTE:
                            if (character.CompareTo('\'') == 0)
                            {
                                toState = transition.To;
                                exitLoop = true;
                                break;
                            }
                            break;
                        case Token.Type.DOUBLE_QUOTE:
                            if (character.CompareTo('\"') == 0)
                            {
                                toState = transition.To;
                                exitLoop = true;
                                break;
                            }
                            break;
                        case Token.Type.TABULATION:
                            if (character.CompareTo('\t') == 0)
                            {
                                toState = transition.To;
                                exitLoop = true;
                                break;
                            }
                            break;
                        case Token.Type.ID:
                            SetC set = sets.Find(x => x.Name == tokenValue);

                            if (set != null)
                            {
                                if (set.Value.Find(x => x.Value.Equals(character.ToString())) != null)
                                {
                                    toState = transition.To;
                                    exitLoop = true;
                                    break;
                                }
                            }
                            break;
                        case Token.Type.NUMBER:
                            if (char.IsDigit(character))
                            {
                                toState = transition.To;
                                exitLoop = true;
                                break;
                            }
                            break;
                        case Token.Type.STR:
                            string str = tokenValue.Substring(1, tokenValue.Length - 2);

                            for (int i = 0; i < str.Length; i++)
                            {
                                entry.Append(charsList[indexChar + i]);
                            }

                            if (str.Equals(entry.ToString()))
                            {
                                indexChar += str.Length - 1;
                                toState = transition.To;
                                exitLoop = true;
                                break;
                            }
                            break;
                        case Token.Type.SYMBOL:
                            if (tokenValue.Equals(character))
                            {
                                toState = transition.To;
                                exitLoop = true;
                                break;
                            }
                            break;
                    }
                    if (exitLoop == true)
                    {
                        break;
                    }
                }

                if (toState != null)
                {
                    indexChar++;
                    state = Evaluation(toState);
                }

                return state;
            }
            return state;
        }

        private void SplitLexeme()
        {
            foreach (char item in charsList)
            {
                Console.WriteLine(item);
            }

            foreach (var item in transitions.OrderBy(x => x.From.StateName))
            {
                Console.WriteLine(item.From.StateName + " -> " + item.To.StateName);
            }

            foreach (var item in sets)
            {
                Console.WriteLine(item.Name);

                foreach (var S in item.Value)
                {
                    Console.WriteLine(S.Value);
                }
            }
        }
    }
}

using OCL1P1.controller;
using OCL1P1.model;
using OCL1P1.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OCL1P1.analyzer
{
    class Interpreter
    {
        private int index;
        private Token preAnalysis;
        private bool syntacticError;
        private int idError;
        private int idSymbol;

        internal List<Token> ListToken { get; set; }
        internal List<Error> ListError { get; set; }
        internal List<Symbol> SymbolTable { get; set; }
        internal List<Expression> Expressions { get; set; }
        internal List<SetC> Sets { get; set; }
        public List<string> RoutesNFA { get; set; }
        public List<string> RoutesTables { get; set; }
        public StringBuilder ConsoleMessage { get; set; }

        public Interpreter(List<Token> listToken)
        {
            ListToken = new List<Token>();
            ListError = new List<Error>();
            SymbolTable = new List<Symbol>();
            Expressions = new List<Expression>();
            Sets = new List<SetC>();
            RoutesNFA = new List<string>();
            RoutesTables = new List<string>();

            ConsoleMessage = new StringBuilder();

            ListToken.AddRange(listToken);
            ListToken.Add(new Token(0, 0, 0, Token.Type.END, "END"));
            index = 0;
            preAnalysis = ListToken[index];
            syntacticError = false;

            idError = 0;
            idSymbol = 0;

            Start();
        }

        private void Start()
        {
            INSTP();
        }

        private void INSTP()
        {
            if (preAnalysis.TypeToken == Token.Type.RESERVED_CONJ
                || preAnalysis.TypeToken == Token.Type.ID
                || preAnalysis.TypeToken == Token.Type.COMMENT
                || preAnalysis.TypeToken == Token.Type.MULTILINE_COMMENT)
            {
                INST();
                INSTP();
            }
        }

        private void INST()
        {
            if (preAnalysis.TypeToken == Token.Type.RESERVED_CONJ)
            {
                DEFCONJ();
            }
            else if (preAnalysis.TypeToken == Token.Type.ID)
            {
                EXPR();
            }
            else if (preAnalysis.TypeToken == Token.Type.COMMENT)
            {
                Parser(Token.Type.COMMENT);
            }
            else if (preAnalysis.TypeToken == Token.Type.MULTILINE_COMMENT)
            {
                Parser(Token.Type.MULTILINE_COMMENT);
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'DEFINITION " +
                    "| EXPRESSION | COMMENT | MULTILINE COMMENT'");
                syntacticError = true;
            }
        }

        private void DEFCONJ()
        {
            if (preAnalysis.TypeToken == Token.Type.RESERVED_CONJ)
            {
                string type = preAnalysis.Value;
                Parser(Token.Type.RESERVED_CONJ);
                Parser(Token.Type.SYMBOL_COLON);
                string name = preAnalysis.Value;
                Parser(Token.Type.ID);
                Parser(Token.Type.ASSIGNMENT_SIGN);
                List<Token> value = ASGMTCONJ();
                Parser(Token.Type.SYMBOL_SEMICOLON);
                AddSymbol(type, name, value);
                Sets.Add(new SetC(name, value));
                updateSet();
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'CONJ'");
            }
        }

        private List<Token> ASGMTCONJ()
        {
            List<Token> tokens = new List<Token>();
            if (preAnalysis.TypeToken == Token.Type.ID
                || preAnalysis.TypeToken == Token.Type.NUMBER
                || preAnalysis.TypeToken == Token.Type.SYMBOL
                || preAnalysis.TypeToken == Token.Type.LINE_BREAK
                || preAnalysis.TypeToken == Token.Type.SINGLE_QUOTE
                || preAnalysis.TypeToken == Token.Type.DOUBLE_QUOTE
                || preAnalysis.TypeToken == Token.Type.TABULATION
                || preAnalysis.TypeToken == Token.Type.CHARACTER_SET)
            {
                tokens.Add(TYPECONJ());
                tokens.AddRange(ASGMTCONJP());
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID | Number | Symbol'");
            }
            return tokens;
        }

        private List<Token> ASGMTCONJP()
        {
            List<Token> tokens = new List<Token>();
            if (preAnalysis.TypeToken == Token.Type.SET_SIGN)
            {
                tokens.Add(preAnalysis);
                Parser(Token.Type.SET_SIGN);
                tokens.AddRange(ASGMTCONJ());
            }
            else if (preAnalysis.TypeToken == Token.Type.SYMBOL_COMMA)
            {
                tokens.Add(preAnalysis);
                Parser(Token.Type.SYMBOL_COMMA);
                tokens.AddRange(ASGMTCONJ());
            }
            return tokens;
        }

        private Token TYPECONJ()
        {
            Token token = null;
            if (preAnalysis.TypeToken == Token.Type.ID)
            {
                token = preAnalysis;
                Parser(Token.Type.ID);
            }
            else if (preAnalysis.TypeToken == Token.Type.NUMBER)
            {
                token = preAnalysis;
                Parser(Token.Type.NUMBER);
            }
            else if (preAnalysis.TypeToken == Token.Type.SYMBOL)
            {
                token = preAnalysis;
                Parser(Token.Type.SYMBOL);
            }
            else if (preAnalysis.TypeToken == Token.Type.LINE_BREAK)
            {
                token = preAnalysis;
                Parser(Token.Type.LINE_BREAK);
            }
            else if (preAnalysis.TypeToken == Token.Type.SINGLE_QUOTE)
            {
                token = preAnalysis;
                Parser(Token.Type.SINGLE_QUOTE);
            }
            else if (preAnalysis.TypeToken == Token.Type.DOUBLE_QUOTE)
            {
                token = preAnalysis;
                Parser(Token.Type.DOUBLE_QUOTE);
            }
            else if (preAnalysis.TypeToken == Token.Type.TABULATION)
            {
                token = preAnalysis;
                Parser(Token.Type.TABULATION);
            }
            else if (preAnalysis.TypeToken == Token.Type.CHARACTER_SET)
            {
                token = preAnalysis;
                Parser(Token.Type.CHARACTER_SET);
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID | Number | Symbol'");
            }
            return token;
        }

        private void EXPR()
        {
            if (preAnalysis.TypeToken == Token.Type.ID)
            {
                Parser(Token.Type.ID);
                OPTEXPR();
                Parser(Token.Type.SYMBOL_SEMICOLON);
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID'");
            }
        }

        private void OPTEXPR()
        {
            if (preAnalysis.TypeToken == Token.Type.ASSIGNMENT_SIGN)
            {
                string type = "EXPR";
                string name = ListToken[index - 1].Value;
                AddSymbol(type, name, null);
                Expressions.Add(new Expression(name, null));

                DEFEXPR();
            }
            else if (preAnalysis.TypeToken == Token.Type.SYMBOL_COLON)
            {
                VALEXPR();
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ASSIGNMENT SIGN | SYMBOL COLON'");
            }
        }

        private void DEFEXPR()
        {
            if (preAnalysis.TypeToken == Token.Type.ASSIGNMENT_SIGN)
            {
                Symbol symbol = GetSymbol(ListToken[index - 1].Value);
                Expression expression = GetExpression(ListToken[index - 1].Value);
                Parser(Token.Type.ASSIGNMENT_SIGN);
                symbol.Value = STRUCEXPR();
                expression.Value = symbol.Value;

                // Thompson's construction
                Thompson nfa = new Thompson(symbol);
                NFAReport nfaReport = new NFAReport();

                nfa.Construction();
                nfaReport.ReportNFA(symbol.Name, nfa.Transitions);

                nfa.RemoveUnnecessaryT();
                nfaReport.ReportNFA(symbol.Name + "Optimized", nfa.Transitions);

                string imageRoute = Directory.GetCurrentDirectory() + "\\" + symbol.Name + ".png";
                string imageRouteOptimized = Directory.GetCurrentDirectory() + "\\" + symbol.Name + "Optimized.png";
                if (File.Exists(imageRoute) && File.Exists(imageRouteOptimized))
                {
                    RoutesNFA.Add(imageRoute);
                    RoutesNFA.Add(imageRouteOptimized);
                }

                // Subset Construction 
                SubsetConstruction subsetConstruction = new SubsetConstruction(nfa.Transitions, nfa.States, nfa.Terminals);
                DFAReport dfaReport = new DFAReport();
                SubsetReport subsetReport = new SubsetReport();

                subsetConstruction.Construction();
                expression.Transitions.AddRange(subsetConstruction.Transitions);

                subsetReport.ReportSubset("T_" + symbol.Name, subsetConstruction.StatesMatrix());
                imageRoute = Directory.GetCurrentDirectory() + "\\" + "T_" + symbol.Name + ".png";
                if (File.Exists(imageRoute))
                {
                    RoutesTables.Add(imageRoute);
                }

                dfaReport.ReportDFA("DFA_" + symbol.Name, subsetConstruction.Transitions);
                imageRoute = Directory.GetCurrentDirectory() + "\\" + "DFA_" + symbol.Name + ".png";
                if (File.Exists(imageRoute))
                {
                    RoutesNFA.Add(imageRoute);
                }
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID'");
            }
        }

        private List<Token> STRUCEXPR()
        {
            List<Token> tokens = new List<Token>();
            if (preAnalysis.TypeToken == Token.Type.CONCATENATION_SIGN
                || preAnalysis.TypeToken == Token.Type.DISJUNCTION_SIGN
                || preAnalysis.TypeToken == Token.Type.QUESTION_MARK_SIGN
                || preAnalysis.TypeToken == Token.Type.ASTERISK_SIGN
                || preAnalysis.TypeToken == Token.Type.PLUS_SIGN
                || preAnalysis.TypeToken == Token.Type.SYMBOL_LEFT_CURLY_BRACKET
                || preAnalysis.TypeToken == Token.Type.STR
                || preAnalysis.TypeToken == Token.Type.NUMBER
                || preAnalysis.TypeToken == Token.Type.SYMBOL
                || preAnalysis.TypeToken == Token.Type.LINE_BREAK
                || preAnalysis.TypeToken == Token.Type.SINGLE_QUOTE
                || preAnalysis.TypeToken == Token.Type.DOUBLE_QUOTE
                || preAnalysis.TypeToken == Token.Type.TABULATION)
            {
                tokens.Add(SYMBEXPR());
                tokens.AddRange(STRUCEXPRP());
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'CONCATENATION SIGN " +
                    "| DISJUNCTION SIGN | QUESTION MARK SIGN | ASTERISK SIGN | PLUS SIGN | SYMBOL LEFT CURLY BRACKET " +
                    "| STRING | NUMBER | SYMBOL | LINE BREAK | SINGLE QUOTE | DOUBLE QUOTE | TABULATION'");
            }
            return tokens;
        }

        private List<Token> STRUCEXPRP()
        {
            List<Token> tokens = new List<Token>();
            if (preAnalysis.TypeToken == Token.Type.CONCATENATION_SIGN
                || preAnalysis.TypeToken == Token.Type.DISJUNCTION_SIGN
                || preAnalysis.TypeToken == Token.Type.QUESTION_MARK_SIGN
                || preAnalysis.TypeToken == Token.Type.ASTERISK_SIGN
                || preAnalysis.TypeToken == Token.Type.PLUS_SIGN
                || preAnalysis.TypeToken == Token.Type.SYMBOL_LEFT_CURLY_BRACKET
                || preAnalysis.TypeToken == Token.Type.STR
                || preAnalysis.TypeToken == Token.Type.NUMBER
                || preAnalysis.TypeToken == Token.Type.SYMBOL
                || preAnalysis.TypeToken == Token.Type.LINE_BREAK
                || preAnalysis.TypeToken == Token.Type.SINGLE_QUOTE
                || preAnalysis.TypeToken == Token.Type.DOUBLE_QUOTE
                || preAnalysis.TypeToken == Token.Type.TABULATION)
            {
                tokens.Add(SYMBEXPR());
                tokens.AddRange(STRUCEXPRP());
            }
            return tokens;
        }

        private Token SYMBEXPR()
        {
            Token token = null;
            if (preAnalysis.TypeToken == Token.Type.CONCATENATION_SIGN)
            {
                token = preAnalysis;
                Parser(Token.Type.CONCATENATION_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.DISJUNCTION_SIGN)
            {
                token = preAnalysis;
                Parser(Token.Type.DISJUNCTION_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.QUESTION_MARK_SIGN)
            {
                token = preAnalysis;
                Parser(Token.Type.QUESTION_MARK_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.ASTERISK_SIGN)
            {
                token = preAnalysis;
                Parser(Token.Type.ASTERISK_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.PLUS_SIGN)
            {
                token = preAnalysis;
                Parser(Token.Type.PLUS_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.SYMBOL_LEFT_CURLY_BRACKET)
            {
                Parser(Token.Type.SYMBOL_LEFT_CURLY_BRACKET);
                token = preAnalysis;
                Parser(Token.Type.ID);
                Parser(Token.Type.SYMBOL_RIGHT_CURLY_BRACKET);
            }
            else if (preAnalysis.TypeToken == Token.Type.STR)
            {
                token = preAnalysis;
                Parser(Token.Type.STR);
            }
            else if (preAnalysis.TypeToken == Token.Type.NUMBER)
            {
                token = preAnalysis;
                Parser(Token.Type.NUMBER);
            }
            else if (preAnalysis.TypeToken == Token.Type.SYMBOL)
            {
                token = preAnalysis;
                Parser(Token.Type.SYMBOL);
            }
            else if (preAnalysis.TypeToken == Token.Type.LINE_BREAK)
            {
                token = preAnalysis;
                Parser(Token.Type.LINE_BREAK);
            }
            else if (preAnalysis.TypeToken == Token.Type.SINGLE_QUOTE)
            {
                token = preAnalysis;
                Parser(Token.Type.SINGLE_QUOTE);
            }
            else if (preAnalysis.TypeToken == Token.Type.DOUBLE_QUOTE)
            {
                token = preAnalysis;
                Parser(Token.Type.DOUBLE_QUOTE);
            }
            else if (preAnalysis.TypeToken == Token.Type.TABULATION)
            {
                token = preAnalysis;
                Parser(Token.Type.TABULATION);
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'CONCATENATION SIGN " +
                    "| DISJUNCTION SIGN | QUESTION MARK SIGN | ASTERISK SIGN | PLUS SIGN | SYMBOL LEFT CURLY BRACKET " +
                    "| STRING | NUMBER | SYMBOL | LINE BREAK | SINGLE QUOTE | DOUBLE QUOTE | TABULATION'");
            }
            return token;
        }

        private void VALEXPR()
        {
            if (preAnalysis.TypeToken == Token.Type.SYMBOL_COLON)
            {
                Expression expression = GetExpression(ListToken[index - 1].Value);
                Parser(Token.Type.SYMBOL_COLON);
                if (expression != null)
                {
                    string lexeme = preAnalysis.Value.Substring(1, preAnalysis.Value.Length - 2);
                    LexemeEvaluation lexemeEvaluation = new LexemeEvaluation(lexeme, expression.Transitions, Sets);
                    ConsoleMessage.AppendLine(expression.Name + ":\n    " + lexemeEvaluation.Initialize() + "\n");
                }
                Parser(Token.Type.STR);
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID'");
            }
        }

        private void Parser(Token.Type type)
        {
            if (syntacticError)
            {
                if (index < ListToken.Count - 1)
                {
                    index++;
                    preAnalysis = ListToken[index];
                    if (preAnalysis.TypeToken == Token.Type.SYMBOL_SEMICOLON)
                    {
                        syntacticError = false;
                    }
                }
            }
            else
            {
                if (preAnalysis.TypeToken != Token.Type.END)
                {
                    if (preAnalysis.TypeToken == type)
                    {
                        index++;
                        preAnalysis = ListToken[index];
                    }
                    else
                    {
                        AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected '" + type + "'");
                        syntacticError = true;
                    }
                }
            }
        }

        private void AddError(int row, int column, string str, string description)
        {
            idError++;
            ListError.Add(new Error(idError, row, column, str, description));
            syntacticError = true;
        }

        private void AddSymbol(string type, string name, List<Token> value)
        {
            idSymbol++;
            SymbolTable.Add(new Symbol(idSymbol, type, name, value));
        }

        private Symbol GetSymbol(string name)
        {
            foreach (Symbol symbol in SymbolTable)
            {
                if (symbol.Name.Equals(name))
                {
                    return symbol;
                }
            }
            return null;
        }

        private Expression GetExpression(string name)
        {
            foreach (Expression expression in Expressions)
            {
                if (expression.Name.Equals(name))
                {
                    return expression;
                }
            }
            return null;
        }

        public void GenerateReports()
        {
            XMLReport xmlReport = new XMLReport();
            xmlReport.ReportSymbolTable(SymbolTable);
        }

        public void updateSet()
        {
            SetC set = Sets.Last();
            List<Token> symbols = new List<Token>();

            for (int i = 0; i < set.Value.Count(); i++)
            {
                if (set.Value[i].TypeToken == Token.Type.SET_SIGN)
                {
                    for (int j = (int)set.Value[i - 1].Value[0]; j <= (int)set.Value[i + 1].Value[0]; j++)
                    {
                        symbols.Add(new Token(0, 0, 0, Token.Type.SYMBOL, Convert.ToChar(j).ToString()));
                    }

                    set.Value.RemoveRange(i - 1, i + 2);
                }
                else if (set.Value[i].TypeToken == Token.Type.SYMBOL_COMMA)
                {

                    set.Value.RemoveAt(i);
                }
                else if (set.Value[i].TypeToken == Token.Type.CHARACTER_SET)
                {
                    string str = set.Value[i].Value.Replace("[:", "").Replace(":]", "");
                    for (int j = 0; j < str.Length; j++)
                    {
                        symbols.Add(new Token(0, 0, 0, Token.Type.SYMBOL, str[j].ToString()));
                    }
                    set.Value.RemoveAt(i);
                }
            }

            set.Value.AddRange(symbols);
        }
    }
}

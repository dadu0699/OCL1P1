using OCL1P1.model;
using OCL1P1.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.analyzer
{
    class SyntacticAnalyzer
    {
        private int index;
        private Token preAnalysis;
        private bool syntacticError;
        private int idError;

        internal List<Token> ListToken;
        internal List<Error> ListError { get; set; }

        public SyntacticAnalyzer(List<Token> listToken)
        {
            ListToken = listToken;
            ListToken.Add(new Token(0, 0, 0, Token.Type.END, "END"));
            index = 0;
            preAnalysis = ListToken[index];
            syntacticError = false;

            idError = 0;
            ListError = new List<Error>();

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
                Parser(Token.Type.RESERVED_CONJ);
                Parser(Token.Type.SYMBOL_COLON);
                Parser(Token.Type.ID);
                Parser(Token.Type.ASSIGNMENT_SIGN);
                ASGMTCONJ();
                Parser(Token.Type.SYMBOL_SEMICOLON);
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'CONJ'");
            }
        }

        private void ASGMTCONJ()
        {
            if (preAnalysis.TypeToken == Token.Type.ID
                || preAnalysis.TypeToken == Token.Type.NUMBER
                || preAnalysis.TypeToken == Token.Type.SYMBOL
                || preAnalysis.TypeToken == Token.Type.LINE_BREAK
                || preAnalysis.TypeToken == Token.Type.SINGLE_QUOTE
                || preAnalysis.TypeToken == Token.Type.DOUBLE_QUOTE
                || preAnalysis.TypeToken == Token.Type.TABULATION
                || preAnalysis.TypeToken == Token.Type.CHARACTER_SET)
            {
                TYPECONJ();
                ASGMTCONJP();
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID | Number | Symbol'");
            }
        }

        private void ASGMTCONJP()
        {
            if (preAnalysis.TypeToken == Token.Type.SET_SIGN)
            {
                Parser(Token.Type.SET_SIGN);
                ASGMTCONJ();
            }
            else if (preAnalysis.TypeToken == Token.Type.SYMBOL_COMMA)
            {
                Parser(Token.Type.SYMBOL_COMMA);
                ASGMTCONJ();
            }
        }

        private void TYPECONJ()
        {
            if (preAnalysis.TypeToken == Token.Type.ID)
            {
                Parser(Token.Type.ID);
            }
            else if (preAnalysis.TypeToken == Token.Type.NUMBER)
            {
                Parser(Token.Type.NUMBER);
            }
            else if (preAnalysis.TypeToken == Token.Type.SYMBOL)
            {
                Parser(Token.Type.SYMBOL);
            }
            else if (preAnalysis.TypeToken == Token.Type.LINE_BREAK)
            {
                Parser(Token.Type.LINE_BREAK);
            }
            else if (preAnalysis.TypeToken == Token.Type.SINGLE_QUOTE)
            {
                Parser(Token.Type.SINGLE_QUOTE);
            }
            else if (preAnalysis.TypeToken == Token.Type.DOUBLE_QUOTE)
            {
                Parser(Token.Type.DOUBLE_QUOTE);
            }
            else if (preAnalysis.TypeToken == Token.Type.TABULATION)
            {
                Parser(Token.Type.TABULATION);
            }
            else if (preAnalysis.TypeToken == Token.Type.CHARACTER_SET)
            {
                Parser(Token.Type.CHARACTER_SET);
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID | Number | Symbol'");
            }
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
                Parser(Token.Type.ASSIGNMENT_SIGN);
                STRUCEXPR();
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID'");
            }
        }

        private void STRUCEXPR()
        {
            if (preAnalysis.TypeToken == Token.Type.CONCATENATION_SIGN
                || preAnalysis.TypeToken == Token.Type.DISJUNCTION_SIGN
                || preAnalysis.TypeToken == Token.Type.QUESTION_MARK_SIGN
                || preAnalysis.TypeToken == Token.Type.ASTERISK_SIGN
                || preAnalysis.TypeToken == Token.Type.PLUS_SIGN
                || preAnalysis.TypeToken == Token.Type.SYMBOL_LEFT_CURLY_BRACKET
                || preAnalysis.TypeToken == Token.Type.STR)
            {
                SYMBEXPR();
                STRUCEXPRP();
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'CONCATENATION SIGN " +
                    "| DISJUNCTION SIGN | QUESTION MARK SIGN | ASTERISK SIGN | PLUS SIGN | SYMBOL LEFT CURLY BRACKET " +
                    "| STRING'");
            }
        }

        private void STRUCEXPRP()
        {
            if (preAnalysis.TypeToken == Token.Type.CONCATENATION_SIGN
                || preAnalysis.TypeToken == Token.Type.DISJUNCTION_SIGN
                || preAnalysis.TypeToken == Token.Type.QUESTION_MARK_SIGN
                || preAnalysis.TypeToken == Token.Type.ASTERISK_SIGN
                || preAnalysis.TypeToken == Token.Type.PLUS_SIGN
                || preAnalysis.TypeToken == Token.Type.SYMBOL_LEFT_CURLY_BRACKET
                || preAnalysis.TypeToken == Token.Type.STR)
            {
                SYMBEXPR();
                STRUCEXPRP();
            }
        }

        private void SYMBEXPR()
        {
            if (preAnalysis.TypeToken == Token.Type.CONCATENATION_SIGN)
            {
                Parser(Token.Type.CONCATENATION_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.DISJUNCTION_SIGN)
            {
                Parser(Token.Type.DISJUNCTION_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.QUESTION_MARK_SIGN)
            {
                Parser(Token.Type.QUESTION_MARK_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.ASTERISK_SIGN)
            {
                Parser(Token.Type.ASTERISK_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.PLUS_SIGN)
            {
                Parser(Token.Type.PLUS_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.SYMBOL_LEFT_CURLY_BRACKET)
            {
                Parser(Token.Type.SYMBOL_LEFT_CURLY_BRACKET);
                Parser(Token.Type.ID);
                Parser(Token.Type.SYMBOL_RIGHT_CURLY_BRACKET);
            }
            else if (preAnalysis.TypeToken == Token.Type.STR)
            {
                Parser(Token.Type.STR);
            }
            else
            {
                AddError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'CONCATENATION SIGN " +
                    "| DISJUNCTION SIGN | QUESTION MARK SIGN | ASTERISK SIGN | PLUS SIGN | SYMBOL LEFT CURLY BRACKET " +
                    "| STRING'");
            }
        }

        private void VALEXPR()
        {
            if (preAnalysis.TypeToken == Token.Type.SYMBOL_COLON)
            {
                Parser(Token.Type.SYMBOL_COLON);
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

        public void GenerateReports()
        {
            XMLReport xmlReport = new XMLReport();
            xmlReport.ReportSyntacticErrors(ListError);
        }
    }
}

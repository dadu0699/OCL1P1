using OCL1P1.model;
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
            index = 0;
            preAnalysis = listToken[index];
            syntacticError = false;

            idError = 0;
            ListError = new List<Error>();

            Start();
        }

        public void Start()
        {
            DEFCONJP();
            EXPRP();
        }

        public void DEFCONJP()
        {
            if (preAnalysis.TypeToken == Token.Type.RESERVED_CONJ)
            {
                DEFCONJ();
                DEFCONJP();
            }
        }

        public void DEFCONJ()
        {
            if (preAnalysis.TypeToken == Token.Type.RESERVED_CONJ)
            {
                parea(Token.Type.RESERVED_CONJ);
                parea(Token.Type.SYMBOL_COLON);
                parea(Token.Type.ID);
                parea(Token.Type.ASSIGNMENT_SIGN);
                ASGMTCONJ();
                parea(Token.Type.SYMBOL_SEMICOLON);
            }
            else
            {
                addError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'CONJ'");
            }
        }

        public void ASGMTCONJ()
        {
            if (preAnalysis.TypeToken == Token.Type.ID
                || preAnalysis.TypeToken == Token.Type.NUMBER
                || preAnalysis.TypeToken == Token.Type.SYMBOL)
            {
                TYPECONJ();
                ASGMTCONJP();
            }
            else
            {
                addError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID | Number | Symbol'");
            }
        }

        public void ASGMTCONJP()
        {
            if (preAnalysis.TypeToken == Token.Type.SET_SIGN)
            {
                parea(Token.Type.SET_SIGN);
                ASGMTCONJ();
            }
            else if (preAnalysis.TypeToken == Token.Type.SYMBOL_COMMA)
            {
                parea(Token.Type.SYMBOL_COMMA);
                ASGMTCONJ();
            }
        }

        public void TYPECONJ()
        {
            if (preAnalysis.TypeToken == Token.Type.ID)
            {
                parea(Token.Type.ID);
            }
            else if (preAnalysis.TypeToken == Token.Type.NUMBER)
            {
                parea(Token.Type.NUMBER);
            }
            else if (preAnalysis.TypeToken == Token.Type.SYMBOL)
            {
                parea(Token.Type.SYMBOL);
            }
            else
            {
                addError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID | Number | Symbol'");
            }
        }

        public void EXPR()
        {
            if (preAnalysis.TypeToken == Token.Type.ID)
            {
                parea(Token.Type.ID);
                OPTEXPR();
                parea(Token.Type.SYMBOL_SEMICOLON);
            }
            else
            {
                addError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID'");
            }
        }

        public void EXPRP()
        {
            if (preAnalysis.TypeToken == Token.Type.ID)
            {
                EXPR();
                EXPRP();
            }
            else
            {
                addError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID'");
            }
        }

        public void OPTEXPR()
        {
            if (preAnalysis.TypeToken == Token.Type.ASSIGNMENT_SIGN)
            {
                DEFEXPR();
            }
            else if (preAnalysis.TypeToken == Token.Type.SYMBOL_SEMICOLON)
            {
                VALEXPR();
            }
            else
            {
                addError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID'");
            }
        }

        public void DEFEXPR()
        {
            if (preAnalysis.TypeToken == Token.Type.ASSIGNMENT_SIGN)
            {
                parea(Token.Type.ASSIGNMENT_SIGN);
                STRUCEXPR();
            }
            else
            {
                addError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID'");
            }
        }

        public void STRUCEXPR()
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
                addError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'CONCATENATION SIGN " +
                    "| DISJUNCTION SIGN | QUESTION MARK SIGN | ASTERISK SIGN | PLUS SIGN | SYMBOL LEFT CURLY BRACKET " +
                    "| STRING'");
            }
        }

        public void STRUCEXPRP()
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

        public void SYMBEXPR()
        {
            if (preAnalysis.TypeToken == Token.Type.CONCATENATION_SIGN)
            {
                parea(Token.Type.CONCATENATION_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.DISJUNCTION_SIGN)
            {
                parea(Token.Type.DISJUNCTION_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.QUESTION_MARK_SIGN)
            {
                parea(Token.Type.QUESTION_MARK_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.ASTERISK_SIGN)
            {
                parea(Token.Type.ASTERISK_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.PLUS_SIGN)
            {
                parea(Token.Type.PLUS_SIGN);
            }
            else if (preAnalysis.TypeToken == Token.Type.SYMBOL_LEFT_CURLY_BRACKET)
            {
                parea(Token.Type.SYMBOL_LEFT_CURLY_BRACKET);
                parea(Token.Type.ID);
                parea(Token.Type.SYMBOL_RIGHT_CURLY_BRACKET);
            }
            else
            {
                addError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'CONCATENATION SIGN " +
                    "| DISJUNCTION SIGN | QUESTION MARK SIGN | ASTERISK SIGN | PLUS SIGN | SYMBOL LEFT CURLY BRACKET " +
                    "| STRING'");
            }
        }

        public void VALEXPR()
        {
            if (preAnalysis.TypeToken == Token.Type.SYMBOL_COLON)
            {
                parea(Token.Type.SYMBOL_COLON);
                parea(Token.Type.STR);
            }
            else
            {
                addError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected 'ID'");
            }
        }

        public void parea(Token.Type type)
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
                if (index < ListToken.Count - 1)
                {
                    if (preAnalysis.TypeToken == type)
                    {
                        index++;
                        preAnalysis = ListToken[index];
                    }
                    else
                    {
                        addError(preAnalysis.Row, preAnalysis.Column, preAnalysis.toStringTypeToken, "Was expected '" + type + "'");
                        syntacticError = true;
                    }
                }
            }
        }

        public void addError(int row, int column, string str, string description)
        {
            idError++;
            ListError.Add(new Error(idError, row, column, str, description));
            syntacticError = true;
        }
    }
}

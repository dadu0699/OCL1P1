using OCL1P1.model;
using OCL1P1.util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OCL1P1.analyzer
{
    class LexicalAnalyzer
    {
        private string auxiliary;
        private int state;
        private int idToken;
        private int idError;
        private int row;
        private int column;

        internal List<Token> ListToken { get; set; }
        internal List<Error> ListError { get; set; }

        public LexicalAnalyzer()
        {
            auxiliary = "";
            state = 0;
            idToken = 0;
            idError = 0;
            row = 1;
            column = 1;

            ListToken = new List<Token>();
            ListError = new List<Error>();
        }

        public void Scanner(string entry)
        {
            char character;
            entry += "#";

            for (int i = 0; i < entry.Length; i++)
            {
                character = entry.ElementAt(i);
                switch (state)
                {
                    case 0:
                        // Reserved Word
                        if (char.IsLetter(character))
                        {
                            state = 1;
                            auxiliary += character;
                        }
                        // Digit
                        else if (char.IsDigit(character))
                        {
                            state = 2;
                            auxiliary += character;
                        }
                        // String
                        else if (character.Equals('"'))
                        {
                            state = 3;
                            auxiliary += character;
                        }
                        // Assignment
                        else if (character.Equals('-'))
                        {
                            state = 4;
                            auxiliary += character;
                        }
                        // Single Line Comment
                        else if (character.Equals('/'))
                        {
                            state = 5;
                            auxiliary += character;
                        }
                        // Multiline Comment
                        else if (character.Equals('<'))
                        {
                            state = 7;
                            auxiliary += character;
                        }
                        // Special Character
                        else if (character.Equals('\\'))
                        {
                            state = 10;
                            auxiliary += character;
                        }
                        // Character Set
                        else if (character.Equals('['))
                        {
                            state = 11;
                            auxiliary += character;
                        }
                        // Blanks and line breaks
                        else if (char.IsWhiteSpace(character))
                        {
                            state = 0;
                            auxiliary = "";
                            // Change row and restart columns in line breaks
                            if (character.CompareTo('\n') == 0)
                            {
                                column = 1;
                                row++;
                            }
                        }
                        // Symbol
                        else if ((i < entry.Length - 1) && !AddSymbol(character))
                        {
                            if (character.Equals('#') && i == (entry.Length - 1))
                            {
                                Console.WriteLine("Lexical analysis completed");
                            }
                            else
                            {
                                Console.WriteLine("Lexical Error: Not Found '" + character + "' in defined patterns");
                                AddError(character.ToString());
                                state = 0;
                            }
                        }
                        break;
                    case 1:
                        if (char.IsLetter(character) || char.IsDigit(character))
                        {
                            state = 1;
                            auxiliary += character;
                        }
                        else
                        {
                            AddWordReserved();
                            i--;
                        }
                        break;
                    case 2:
                        if (char.IsDigit(character))
                        {
                            state = 2;
                            auxiliary += character;
                        }
                        else
                        {
                            AddToken(Token.Type.NUMBER);
                            i--;
                        }
                        break;
                    case 3:
                        if (!character.Equals('"'))
                        {
                            state = 3;
                            auxiliary += character;
                        }
                        else
                        {
                            auxiliary += character;
                            AddToken(Token.Type.STR);
                        }
                        break;
                    case 4:
                        if (character.Equals('>'))
                        {
                            auxiliary += character;
                            AddToken(Token.Type.ASSIGNMENT_SIGN);
                        }
                        else
                        {
                            AddSymbol(auxiliary.ElementAt(0));
                            i--;
                        }
                        break;
                    case 5:
                        if (character.Equals('/'))
                        {
                            state = 6;
                            auxiliary += character;
                        }
                        else
                        {
                            auxiliary = "";
                            AddSymbol('/');
                            i--;
                        }
                        break;
                    case 6:
                        if (!character.Equals('\n'))
                        {
                            state = 6;
                            auxiliary += character;
                        }
                        else
                        {
                            auxiliary += character;
                            AddToken(Token.Type.COMMENT);
                        }
                        break;
                    case 7:
                        if (character.Equals('!'))
                        {
                            state = 8;
                            auxiliary += character;
                        }
                        else
                        {
                            auxiliary = "";
                            AddSymbol('<');
                            i--;
                        }
                        break;
                    case 8:
                        if (!character.Equals('!'))
                        {
                            state = 8;
                            auxiliary += character;
                        }
                        else
                        {
                            state = 9;
                            auxiliary += character;
                        }
                        break;
                    case 9:
                        if (!character.Equals('>'))
                        {
                            state = 8;
                            auxiliary += character;
                        }
                        else
                        {
                            auxiliary += character;
                            AddToken(Token.Type.MULTILINE_COMMENT);
                        }
                        break;
                    case 10:
                        if (character.Equals('n'))
                        {
                            auxiliary += character;
                            AddToken(Token.Type.LINE_BREAK);
                        }
                        else if (character.Equals('\''))
                        {
                            auxiliary += character;
                            AddToken(Token.Type.SINGLE_QUOTE);
                        }
                        else if (character.Equals('\"'))
                        {
                            auxiliary += character;
                            AddToken(Token.Type.DOUBLE_QUOTE);
                        }
                        else if (character.Equals('t'))
                        {
                            auxiliary += character;
                            AddToken(Token.Type.TABULATION);
                        }
                        else
                        {
                            auxiliary = "";
                            AddSymbol('\\');
                            i--;
                        }
                        break;
                    case 11:
                        if (character.Equals(':'))
                        {
                            state = 12;
                            auxiliary += character;
                        }
                        else
                        {
                            auxiliary = "";
                            AddSymbol('[');
                            i--;
                        }
                        break;
                    case 12:
                        if (!character.Equals(':'))
                        {
                            state = 12;
                            auxiliary += character;
                        }
                        else
                        {
                            state = 13;
                            auxiliary += character;
                        }
                        break;
                    case 13:
                        if (!character.Equals(']'))
                        {
                            state = 12;
                            auxiliary += character;
                        }
                        else
                        {
                            auxiliary += character;
                            AddToken(Token.Type.CHARACTER_SET);
                        }
                        break;
                }
                column++;
            }
        }

        private bool AddSymbol(char character)
        {
            if (character.Equals('{'))
            {
                auxiliary += character;
                AddToken(Token.Type.SYMBOL_LEFT_CURLY_BRACKET);
                return true;
            }
            else if (character.Equals('}'))
            {
                auxiliary += character;
                AddToken(Token.Type.SYMBOL_RIGHT_CURLY_BRACKET);
                return true;
            }
            else if (character.Equals(':'))
            {
                auxiliary += character;
                AddToken(Token.Type.SYMBOL_COLON);
                return true;
            }
            else if (character.Equals(';'))
            {
                auxiliary += character;
                AddToken(Token.Type.SYMBOL_SEMICOLON);
                return true;
            }
            else if (character.Equals(','))
            {
                auxiliary += character;
                AddToken(Token.Type.SYMBOL_COMMA);
                return true;
            }
            else if (character.Equals('.'))
            {
                auxiliary += character;
                AddToken(Token.Type.CONCATENATION_SIGN);
                return true;
            }
            else if (character.Equals('|'))
            {
                auxiliary += character;
                AddToken(Token.Type.DISJUNCTION_SIGN);
                return true;
            }
            else if (character.Equals('?'))
            {
                auxiliary += character;
                AddToken(Token.Type.QUESTION_MARK_SIGN);
                return true;
            }
            else if (character.Equals('*'))
            {
                auxiliary += character;
                AddToken(Token.Type.ASTERISK_SIGN);
                return true;
            }
            else if (character.Equals('+'))
            {
                auxiliary += character;
                AddToken(Token.Type.PLUS_SIGN);
                return true;
            }
            else if (character.Equals('~'))
            {
                auxiliary += character;
                AddToken(Token.Type.SET_SIGN);
                return true;
            }
            else
            {
                for (int i = 32; i <= 125; i++)
                {
                    if (!char.IsDigit(Convert.ToChar(i)) && !char.IsLetter(Convert.ToChar(i)))
                    {
                        if ((int)character == i)
                        {
                            auxiliary += character;
                            AddToken(Token.Type.SYMBOL);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void AddWordReserved()
        {
            if (auxiliary.Equals("CONJ", StringComparison.InvariantCultureIgnoreCase))
            {
                AddToken(Token.Type.RESERVED_CONJ);
            }
            else
            {
                AddToken(Token.Type.ID);
            }
        }

        private void AddToken(Token.Type type)
        {
            idToken++;
            ListToken.Add(new Token(idToken, row, column - auxiliary.Length, type, auxiliary));
            auxiliary = "";
            state = 0;
        }

        private void AddError(String chain)
        {
            idError++;
            ListError.Add(new Error(idError, row, column, chain, "Unknown pattern"));
        }

        public void GenerateReportToken()
        {
            XMLReport xmlReport = new XMLReport();
            xmlReport.ReportToken(ListToken);
        }

        public void GenerateReportLexicalErrors()
        {
            XMLReport xmlReport = new XMLReport();
            xmlReport.ReportLexicalErrors(ListError);
        }

        public void GenerateReportLexicalErrorsPDF()
        {
            PDFReport pdfReport = new PDFReport();
            pdfReport.GeneratePDF(ListError);
        }
    }
}

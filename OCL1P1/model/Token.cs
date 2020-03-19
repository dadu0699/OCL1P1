using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.model
{
    class Token
    {
        public enum Type
        {
            RESERVED_CONJ,
            SYMBOL_LEFT_CURLY_BRACKET, // {
            SYMBOL_RIGHT_CURLY_BRACKET, // }
            SYMBOL_COLON, // :
            SYMBOL_SEMICOLON, // ;
            SYMBOL_COMMA, // ,
            PERCENT_SIGN, // %
            CONCATENATION_SIGN, // .
            DISJUNCTION_SIGN, // |
            QUESTION_MARK_SIGN, // ?
            ASTERISK_SIGN, // *
            PLUS_SIGN, // +
            SET_SIGN, // ~
            ASSIGNMENT_SIGN, // ->
            COMMENT, // //
            MULTILINE_COMMENT, // <!!>
            LINE_BREAK, // \n
            SINGLE_QUOTE, // \'
            DOUBLE_QUOTE, // \"
            TABULATION, // \t
            CHARACTER_SET, // [::]
            ID,
            NUMBER,
            STR, // " "
            SYMBOL, // ASCII 35 - 125
            EPSILON, // ε
            END // END
        }

        private int idToken;
        private int row;
        private int column;
        private Type typeToken;
        private string value;

        public int IdToken { get => idToken; set => idToken = value; }
        public int Row { get => row; set => row = value; }
        public int Column { get => column; set => column = value; }
        public string Value { get => value; set => this.value = value; }
        public Type TypeToken { get => typeToken; set => typeToken = value; }
        public string toStringTypeToken
        {
            get
            {
                switch (typeToken)
                {
                    case Type.RESERVED_CONJ:
                        return "RESERVED CONJ";
                    case Type.SYMBOL_LEFT_CURLY_BRACKET:
                        return "SYMBOL LEFT CURLY BRACKET"; // {
                    case Type.SYMBOL_RIGHT_CURLY_BRACKET:
                        return "SYMBOL RIGHT CURLY BRACKET"; // }
                    case Type.SYMBOL_COLON: // :
                        return "SYMBOL COLON";
                    case Type.SYMBOL_SEMICOLON:
                        return "SYMBOL SEMICOLON"; // ;
                    case Type.SYMBOL_COMMA:
                        return "SYMBOL COMMA"; // ,
                    case Type.PERCENT_SIGN:
                        return "PERCENT SIGN"; // %
                    case Type.CONCATENATION_SIGN:
                        return "CONCATENATION SIGN"; // .
                    case Type.DISJUNCTION_SIGN:
                        return "DISJUNCTION SIGN"; // |
                    case Type.QUESTION_MARK_SIGN:
                        return "QUESTION MARK SIGN"; // ?
                    case Type.ASTERISK_SIGN:
                        return "ASTERISK SIGN"; // *
                    case Type.PLUS_SIGN:
                        return "PLUS SIGN"; // +
                    case Type.SET_SIGN:
                        return "SET SIGN"; // ~
                    case Type.ASSIGNMENT_SIGN:
                        return "ASSIGNMENT SIGN"; // ->
                    case Type.COMMENT:
                        return "COMMENT"; // //
                    case Type.MULTILINE_COMMENT:
                        return "MULTILINE COMMENT"; // <!!>
                    case Type.LINE_BREAK:
                        return "LINE BREAK"; // \n
                    case Type.SINGLE_QUOTE:
                        return "SINGLE QUOTE"; // \'
                    case Type.DOUBLE_QUOTE:
                        return "DOUBLE QUOTE"; // \""
                    case Type.TABULATION:
                        return "TABULATION"; // \t
                    case Type.CHARACTER_SET:
                        return "CHARACTER SET"; // [::]
                    case Type.ID:
                        return "ID";
                    case Type.NUMBER:
                        return "NUMBER";
                    case Type.STR:
                        return "STR"; // " "
                    case Type.SYMBOL:
                        return "SYMBOL"; // ASCII 35 - 125
                    case Type.EPSILON:
                        return "EPSILON"; // ε
                    case Type.END:
                        return "END OF FILE"; // EOF
                    default:
                        return "Unknown";
                }
            }
        }

        public Token(int idToken, int row, int column, Type typeToken, string value)
        {
            this.IdToken = idToken;
            this.Row = row;
            this.Column = column;
            this.TypeToken = typeToken;
            this.Value = value;
        }
    }
}

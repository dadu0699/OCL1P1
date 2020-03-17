using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.model
{
    class Symbol
    {
        private int idSymbol;
        private string type;
        private string name;
        private List<Token> value;

        public Symbol(int idSymbol, string type, string name, List<Token> value)
        {
            this.IdSymbol = idSymbol;
            this.Type = type;
            this.Name = name;
            this.Value = value;
        }

        public int IdSymbol { get => idSymbol; set => idSymbol = value; }
        public string Type { get => type; set => type = value; }
        public string Name { get => name; set => name = value; }
        internal List<Token> Value { get => value; set => this.value = value; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.model
{
    class SetC
    {
        private string name;
        private List<Token> value;

        public SetC(string name, List<Token> value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get => name; set => name = value; }
        internal List<Token> Value { get => value; set => this.value = value; }
    }
}

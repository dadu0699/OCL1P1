using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.model
{
    class Expression
    {
        private string name;
        private List<Token> value;
        private List<Transition> transitions;

        public Expression(string name, List<Token> value)
        {
            this.Name = name;
            this.Value = value;
            Transitions = new List<Transition>();
        }

        public string Name { get => name; set => name = value; }
        internal List<Token> Value { get => value; set => this.value = value; }
        internal List<Transition> Transitions { get => transitions; set => transitions = value; }
    }
}

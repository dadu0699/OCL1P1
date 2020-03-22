using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.model
{
    class Subset
    {
        private string name;
        private List<State> states;

        public Subset(string name, List<State> states)
        {
            this.Name = name;
            this.States = states;
        }

        public string Name { get => name; set => name = value; }
        internal List<State> States { get => states; set => states = value; }
    }
}

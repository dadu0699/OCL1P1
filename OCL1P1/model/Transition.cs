using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCL1P1.model
{
    class Transition
    {
        private State state;
        private Token token;

        public Transition(State state, Token token)
        {
            this.State = state;
            this.Token = token;
        }

        internal State State { get => state; set => state = value; }
        internal Token Token { get => token; set => token = value; }
    }
}

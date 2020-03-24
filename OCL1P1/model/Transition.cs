namespace OCL1P1.model
{
    class Transition
    {
        private State from;
        private Token token;
        private State to;

        public Transition(State from, Token token, State to)
        {
            this.From = from;
            this.Token = token;
            this.To = to;
        }

        internal State From { get => from; set => from = value; }
        internal Token Token { get => token; set => token = value; }
        internal State To { get => to; set => to = value; }
    }
}

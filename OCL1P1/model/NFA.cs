namespace OCL1P1.model
{
    class NFA
    {
        private Transition initial;
        private Transition acceptance;

        public NFA(Transition initial, Transition acceptance)
        {
            this.Initial = initial;
            this.Acceptance = acceptance;
        }

        internal Transition Initial { get => initial; set => initial = value; }
        internal Transition Acceptance { get => acceptance; set => acceptance = value; }
    }
}

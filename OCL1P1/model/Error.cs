namespace OCL1P1.model
{
    class Error
    {
        private int idError;
        private int row;
        private int column;
        private string character;
        private string description;

        public Error(int idError, int row, int column, string character, string description)
        {
            this.IdError = idError;
            this.Row = row;
            this.Column = column;
            this.Character = character;
            this.Description = description;
        }

        public int IdError { get => idError; set => idError = value; }
        public int Row { get => row; set => row = value; }
        public int Column { get => column; set => column = value; }
        public string Character { get => character; set => character = value; }
        public string Description { get => description; set => description = value; }
    }
}


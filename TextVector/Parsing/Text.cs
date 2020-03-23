namespace TextVector.Parsing
{
    public readonly struct Text
    {
        public Text(int x, int y, int textId, string value)
        {
            X = x;
            Y = y;
            TextId = textId;
            Value = value;
        }

        public int X { get; }
        public int Y { get; }
        public int TextId { get; }
        public string Value { get; }

    }
}
using System.Collections.Generic;

namespace TextVector
{
    public readonly struct Node
    {
        public Node(int x, int y, char c)
        {
            (X, Y, C) = (x, y, c);
            Nodes = new List<Node>();
        }

        public int X { get; }
        public int Y { get; }
        public char C { get; }
        public IList<Node> Nodes { get; }

        public override string ToString()
        {
            return $"{X},{Y} '{C}' ←{Nodes.Count}→";
        }
    }
}
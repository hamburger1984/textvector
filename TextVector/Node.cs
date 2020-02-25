using System.Collections.Generic;

namespace TextVector
{
    public readonly struct Node
    {
        public Node(int x, int y, int figureId, char c)
        {
            (X, Y, FigureId, C) = (x, y, figureId, c);
            Nodes = new List<Node>();
        }


        public int X { get; }
        public int Y { get; }
        public int FigureId { get; }
        public char C { get; }
        public IList<Node> Nodes { get; }

        public override string ToString()
        {
            return $"{FigureId}: {X},{Y} '{C}' ←{Nodes.Count}→";
        }
    }
}
using System.Collections.Generic;

namespace TextVector.Parsing
{
    public readonly struct Node
    {
        public Node(int x, int y, int figureId, char c, TraceDirection direction)
        {
            (X, Y, FigureId, C) = (x, y, figureId, c);
            Direction = direction;
            Nodes = new List<Node>();
        }

        public static implicit operator Node((int x, int y, int figureId, char c, TraceDirection direction) nodeTuple)
        {
            return new Node(nodeTuple.x, nodeTuple.y, nodeTuple.figureId, nodeTuple.c, nodeTuple.direction);
        }

        // public static implicit operator Node?((int x, int y, int figureId, char c, TraceDirection direction) nodeTuple)
        // {
        //     return new Node(nodeTuple.x, nodeTuple.y, nodeTuple.figureId, nodeTuple.c, nodeTuple.direction);
        // }

        public int X { get; }
        public int Y { get; }
        public int FigureId { get; }
        public char C { get; }
        public TraceDirection Direction { get; }
        public IList<Node> Nodes { get; }

        public override string ToString()
        {
            return $"{FigureId}: {X},{Y} '{C}' ←{Nodes.Count}→";
        }
    }
}
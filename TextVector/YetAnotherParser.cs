using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextVector
{
    public class YetAnotherParser
    {
        private const string HorizontalLineChars = "-<>";
        private const string VerticalLineChars = "|v^";
        private const string DiagonalLineCharsUp = "/v^";
        private const string DiagonalLineCharsDown = @"\v^";
        private readonly TextBuffer _buffer;

        private const string LineChars = @"-|+*/\.'<>v^oO";

        public YetAnotherParser(IReadOnlyList<string> lines)
        {
            _buffer = new TextBuffer(lines);
        }

        public string ParseToText()
        {
            var result = new StringBuilder();
            foreach (var figure in ParseFigures()) DumpGraph(result, figure);

            return result.ToString();
        }

        private void DumpGraph(StringBuilder sb, Node node, string suffix = "")
        {
            sb.AppendLine($"{node.FigureId}{suffix} ({node.X},{node.Y},{node.C})");

            var isFork = node.Nodes.Count > 1;
            var i = 1;
            foreach (var n in node.Nodes)
                DumpGraph(sb, n, isFork ? $"{suffix}.{i++}" : suffix);
        }


        public string ParseToSvg(string? filename = null)
        {
            var svg = new SvgGenerator().ToSvg(ParseFigures(), _buffer.Width, _buffer.Height);
            if (!string.IsNullOrEmpty(filename))
                File.WriteAllText(svg, filename);
            return svg;
        }

        private IEnumerable<Node> ParseFigures()
        {
            var figureId = 1;
            for (var y = 0; y < _buffer.Height; y++)
                for (var x = 0; x < _buffer.Width; x++)
                {
                    if (_buffer.IsFigure(x, y) != 0) continue;

                    var c = _buffer[x, y];
                    if (LineChars.Contains(c))
                    {
                        var node = new Node(x, y, figureId, c, TraceDirection.Horizontal | TraceDirection.Right);
                        TraceFrom(node);

                        if (node.Nodes.Any())
                        {
                            yield return node;
                            figureId++;
                        }
                    }
                }
        }

        private void TraceFrom(Node node)
        {
            foreach (var direction in LinesFrom(node.X, node.Y, node.FigureId, node.C))
            {
                var next = TraceLine(node.X, node.Y, node.FigureId, node.C, direction);
                if (next != null)
                {
                    var n = next.Value;
                    node.Nodes.Add(n);
                    TraceFrom(n);
                }
            }
        }

        private Node? TraceLine(int x, int y, int figureId, char c, TraceDirection direction)
        {
            _buffer.SetFigure(x, y, figureId);

            if (direction.HasFlag(TraceDirection.Horizontal))
                return TraceHorizontal(x, y, figureId, c, direction.HasFlag(TraceDirection.Right), direction);
            if (direction.HasFlag(TraceDirection.Vertical))
                return TraceVertical(x, y, figureId, c, direction.HasFlag(TraceDirection.Down), direction);
            if (direction.HasFlag(TraceDirection.Diagonal))
                return TraceDiagonal(x, y, figureId, c, direction.HasFlag(TraceDirection.Right),
                    direction.HasFlag(TraceDirection.Down), direction);
            return null;
        }

        private (bool, Node?) AlreadyTaken(int x, int y, int figureId, char c, TraceDirection direction)
        {
            var existing = _buffer.IsFigure(x, y);
            if (existing == figureId)
                return (true, new Node(x, y, figureId, c, direction));
            return (existing != 0, null);
        }

        private Node? TraceHorizontal(int x, int y, int figureId, char c, bool right, TraceDirection direction)
        {
            // step
            x = right ? x + 1 : x - 1;
            var next = _buffer[x, y];

            switch (AlreadyTaken(x, y, figureId, next, direction))
            {
                case (true, var closingNode):
                    return closingNode;
            }


            // Node, intersection,  soft bend
            if (next == '*' || next == '+' || next == '|' || next == '.' || next == '\'' || next == '<' || next == '>')
            {
                _buffer.SetFigure(x, y, figureId);
                return new Node(x, y, figureId, next, direction);
            }

            if (next == '-')
            {
                _buffer.SetFigure(x, y, figureId);

                //if (c != next)
                //    return new Node(x, y, next);

                // Fork
                if (LinesFrom(x, y, figureId, next).Count() > 1)
                    return new Node(x, y, figureId, next, direction);

                // keep going
                return TraceHorizontal(x, y, figureId, next, right, direction) ?? new Node(x, y, figureId, next, direction);
            }

            return null;
        }

        private Node? TraceVertical(int x, int y, int figureId, char c, bool down, TraceDirection direction)
        {
            // step
            y = down ? y + 1 : y - 1;
            var next = _buffer[x, y];

            switch (AlreadyTaken(x, y, figureId, next, direction))
            {
                case (true, var closingNode):
                    return closingNode;
            }

            // Node, intersection, soft bend
            if (next == '*' || next == '+' || next == '-' || next == '/' || next == '\\' || next == '\'' ||
                down && next == '\'' || !down && next == '.' ||
                next == '^' || next == 'v')
            {
                _buffer.SetFigure(x, y, figureId);
                return new Node(x, y, figureId, next, direction);
            }

            if (next == '|')
            {
                _buffer.SetFigure(x, y, figureId);

                //if (c != next)
                //    return new Node(x, y, next);

                // Fork
                if (LinesFrom(x, y, figureId, next).Count() > 1)
                    return new Node(x, y, figureId, next, direction);

                // keep going
                return TraceVertical(x, y, figureId, next, down, direction) ?? new Node(x, y, figureId, next, direction);
            }

            return null;
        }

        private Node? TraceDiagonal(int x, int y, int figureId, char c, bool right, bool down, TraceDirection direction)
        {
            // step
            x = right ? x + 1 : x - 1;
            y = down ? y + 1 : y - 1;

            var next = _buffer[x, y];

            switch (AlreadyTaken(x, y, figureId, next, direction))
            {
                case (true, var closingNode):
                    return closingNode;
            }


            // Node, bend
            if (next == '*' || next == '+' ||
                down && next == '\'' || !down && next == '.' ||
                next == 'v' || next == '^' || next == '>' || next == '<' ||
                next == '-' || next == '|')
            {
                _buffer.SetFigure(x, y, figureId);
                return new Node(x, y, figureId, next, direction);
            }

            if (down == right && next == '\\' || down != right && next == '/')
            {
                _buffer.SetFigure(x, y, figureId);

                //if (c != next)
                //    return new Node(x, y, next);

                // Fork
                if (LinesFrom(x, y, figureId, next).Count() > 1)
                    return new Node(x, y, figureId, next, direction);

                // keep going
                return TraceDiagonal(x, y, figureId, next, right, down, direction) ?? new Node(x, y, figureId, next, direction);
            }

            return null;
        }

        private IEnumerable<TraceDirection> LinesFrom(int x, int y, int figureId, char c)
        {
            if (_buffer.IsFigure(x, y - 1) == 0 && VerticalLineChars.Contains(_buffer[x, y - 1]))
                yield return TraceDirection.Vertical | TraceDirection.Up;

            if (_buffer.IsFigure(x + 1, y - 1) == 0 && DiagonalLineCharsUp.Contains(_buffer[x + 1, y - 1]))
                yield return TraceDirection.Diagonal | TraceDirection.Right | TraceDirection.Up;

            if (_buffer.IsFigure(x + 1, y) == 0 && HorizontalLineChars.Contains(_buffer[x + 1, y]))
                yield return TraceDirection.Horizontal | TraceDirection.Right;

            if (_buffer.IsFigure(x + 1, y + 1) == 0 && DiagonalLineCharsDown.Contains(_buffer[x + 1, y + 1]))
                yield return TraceDirection.Diagonal | TraceDirection.Right | TraceDirection.Down;

            if (_buffer.IsFigure(x, y + 1) == 0 && VerticalLineChars.Contains(_buffer[x, y + 1]))
                yield return TraceDirection.Vertical | TraceDirection.Down;

            if (_buffer.IsFigure(x - 1, y + 1) == 0 && DiagonalLineCharsUp.Contains(_buffer[x - 1, y + 1]))
                yield return TraceDirection.Diagonal | TraceDirection.Left | TraceDirection.Down;

            if (_buffer.IsFigure(x - 1, y) == 0 && HorizontalLineChars.Contains(_buffer[x - 1, y]))
                yield return TraceDirection.Horizontal | TraceDirection.Left;

            if (_buffer.IsFigure(x - 1, y - 1) == 0 && DiagonalLineCharsDown.Contains(_buffer[x - 1, y - 1]))
                yield return TraceDirection.Diagonal | TraceDirection.Left | TraceDirection.Up;

            /*
            -1,-1  0,-1  1,-1
            -1, 0  0, 0  1, 0
            -1, 1  0, 1  1, 1
            */
        }
    }
}
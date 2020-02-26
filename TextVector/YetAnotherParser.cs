using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextVector
{
    public class YetAnotherParser
    {
        private const string LineChars = @"-|+*/\.'<>vV^oO";
        private const string LineCharsHorizontal = "-<>oO";
        private const string LineCharsVerticalUp = "|+vV^.oO";
        private const string LineCharsVerticalDown = "|+vV^'oO";
        private const string LineCharsDiagonalDownRight = @"\vV^";
        private const string LineCharsDiagonalUpRight = "/vV^";

        private const string TraceNodesHorizontal = @"*+|.'<>oO";
        private const string TraceNodesVertical = @"*+-/\^vVoO";
        private const string TraceNodesDiagonal = "+vV^<>-|";
        private const char TraceDownBend = '\'';
        private const char TraceUpBend = '.';

        private const char TraceContinueHorizontal = '-';
        private const char TraceContinueVertical = '|';
        private const char TraceContinueDiagonalDownRight = '\\';
        private const char TraceContinueDiagonalUpRight = '/';
        private readonly TextBuffer _buffer;

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
            var defaultDirection = TraceDirection.Horizontal | TraceDirection.Right;
            for (var y = 0; y < _buffer.Height; y++)
                for (var x = 0; x < _buffer.Width; x++)
                {
                    if (_buffer.IsFigure(x, y) != 0) continue;

                    var c = _buffer[x, y];
                    if (LineChars.Contains(c))
                    {
                        var node = new Node(x, y, figureId, c, defaultDirection);
                        TraceFrom(node, defaultDirection);

                        if (node.Nodes.Any())
                        {
                            yield return node;
                            figureId++;
                        }
                    }
                }
        }

        private void TraceFrom(Node node, TraceDirection direction)
        {
            foreach (var nextDirection in LinesFrom(node.X, node.Y, node.FigureId, node.C, direction))
            {
                var next = TraceLine(node.X, node.Y, node.FigureId, node.C, nextDirection);
                if (next != null)
                {
                    var n = next.Value;
                    node.Nodes.Add(n);
                    TraceFrom(n, nextDirection);
                }
            }
        }

        private Node? TraceLine(int x, int y, int figureId, char c, TraceDirection direction)
        {
            _buffer.SetFigure(x, y, figureId);
            return TraceDirectionalLine(x, y, figureId, direction);
        }

        private (bool, Node?) AlreadyTaken(int x, int y, int figureId, char c, TraceDirection direction)
        {
            var existing = _buffer.IsFigure(x, y);
            if (existing == figureId)
                return (true, new Node(x, y, figureId, c, direction));
            return (existing != 0, null);
        }

        private Node? TraceDirectionalLine(int x, int y, int figureId, TraceDirection direction)
        {
            var down = direction.HasFlag(TraceDirection.Down);
            var right = direction.HasFlag(TraceDirection.Right);
            var vertical = direction.HasFlag(TraceDirection.Vertical);
            var horizontal = direction.HasFlag(TraceDirection.Horizontal);

            // step
            if (!vertical)
                x = right ? x + 1 : x - 1;
            if (!horizontal)
                y = down ? y + 1 : y - 1;

            var next = _buffer[x, y];

            switch (AlreadyTaken(x, y, figureId, next, direction))
            {
                case (true, var closingNode):
                    return closingNode;
            }

            // Node, bend
            if (RequiresNode(next, down, horizontal, vertical))
            {
                _buffer.SetFigure(x, y, figureId);
                return new Node(x, y, figureId, next, direction);
            }

            if (!ContinueTracing(next, down, right, horizontal, vertical)) return null;

            _buffer.SetFigure(x, y, figureId);

            // don't know...
            //if (c != next)
            //    return new Node(x, y, next);

            // Fork (>1 outgoing lines)
            if (LinesFrom(x, y, figureId, next, direction).Skip(1).Any())
                return new Node(x, y, figureId, next, direction);

            // keep going
            return TraceDirectionalLine(x, y, figureId, direction) ??
                   new Node(x, y, figureId, next, direction);
        }

        private bool ContinueTracing(in char next, in bool down, in bool right, in bool horizontal, in bool vertical)
        {
            if (horizontal)
                return next == TraceContinueHorizontal;

            if (vertical)
                return next == TraceContinueVertical;

            // diagonal
            return down == right && next == TraceContinueDiagonalDownRight ||
                   down != right && next == TraceContinueDiagonalUpRight;
        }


        private bool RequiresNode(in char next, in bool down, in bool horizontal, in bool vertical)
        {
            if (horizontal)
                return TraceNodesHorizontal.Contains(next);

            if (vertical)
                return TraceNodesVertical.Contains(next) ||
                       down && next == TraceDownBend ||
                       !down && next == TraceUpBend;

            // diagonal
            return TraceNodesDiagonal.Contains(next) ||
                   down && next == TraceDownBend ||
                   !down && next == TraceUpBend;
        }

        private IEnumerable<TraceDirection> LinesFrom(int x, int y, int figureId, char c, TraceDirection direction)
        {
            if (_buffer.IsFigure(x, y - 1) == 0 && c != '.' && LineCharsVerticalUp.Contains(_buffer[x, y - 1]))
                yield return TraceDirection.Vertical | TraceDirection.Up;

            if (_buffer.IsFigure(x + 1, y - 1) == 0 && LineCharsDiagonalUpRight.Contains(_buffer[x + 1, y - 1]))
                yield return TraceDirection.Diagonal | TraceDirection.Right | TraceDirection.Up;

            if (_buffer.IsFigure(x + 1, y) == 0 && LineCharsHorizontal.Contains(_buffer[x + 1, y]))
                yield return TraceDirection.Horizontal | TraceDirection.Right;

            if (_buffer.IsFigure(x + 1, y + 1) == 0 && LineCharsDiagonalDownRight.Contains(_buffer[x + 1, y + 1]))
                yield return TraceDirection.Diagonal | TraceDirection.Right | TraceDirection.Down;

            if (_buffer.IsFigure(x, y + 1) == 0 && c != '\'' && LineCharsVerticalDown.Contains(_buffer[x, y + 1]))
                yield return TraceDirection.Vertical | TraceDirection.Down;

            if (_buffer.IsFigure(x - 1, y + 1) == 0 && LineCharsDiagonalUpRight.Contains(_buffer[x - 1, y + 1]))
                yield return TraceDirection.Diagonal | TraceDirection.Left | TraceDirection.Down;

            if (_buffer.IsFigure(x - 1, y) == 0 && LineCharsHorizontal.Contains(_buffer[x - 1, y]))
                yield return TraceDirection.Horizontal | TraceDirection.Left;

            if (_buffer.IsFigure(x - 1, y - 1) == 0 && LineCharsDiagonalDownRight.Contains(_buffer[x - 1, y - 1]))
                yield return TraceDirection.Diagonal | TraceDirection.Left | TraceDirection.Up;

            /*
            -1,-1  0,-1  1,-1
            -1, 0  0, 0  1, 0
            -1, 1  0, 1  1, 1
            */
        }
    }
}
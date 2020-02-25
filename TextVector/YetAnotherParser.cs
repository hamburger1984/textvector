using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace TextVector
{
    public class YetAnotherParser
    {
        private readonly TextBuffer _buffer;
        private readonly List<Node> _figures;

        public YetAnotherParser(IReadOnlyList<string> lines)
        {
            _buffer = new TextBuffer(lines, neighborhood: 2);
            _figures = new List<Node>();
        }

        private readonly char[] _lineChars = new[]
        {
            '-', '|',
            '+', '*',
            '/', '\\',
            '.', '\''
        };

        public string Parse()
        {
            var messages = new StringBuilder();

            _figures.Clear();

            for (var y = 0; y < _buffer.Height; y++)
                for (var x = 0; x < _buffer.Width; x++)
                {
                    if (_buffer.IsTaken(x, y)) continue;

                    var c = _buffer[x, y];
                    if (_lineChars.Contains(c))
                    {

                        var node = new Node(x, y, c);
                        TraceFrom(node);

                        if (node.Nodes.Any())
                            _figures.Add(node);
                    }
                }

            var result = new StringBuilder();
            var prefix = 1;
            foreach (var figure in _figures)
            {
                DumpGraph(result, $"{prefix++}", figure);
            }

            //return messages.ToString();
            return result.ToString();
        }

        private void TraceFrom(Node node)
        {
            foreach (var direction in LinesFrom(node.X, node.Y, node.C))
            {
                var next = TraceLine(node.X, node.Y, node.C, direction);
                if (next != null)
                {
                    var n = next.Value;
                    node.Nodes.Add(n);
                    TraceFrom(n);
                }
            }
        }


        private void DumpGraph(StringBuilder sb, string prefix, Node node)
        {
            sb.AppendLine($"{prefix} ({node.X},{node.Y},{node.C})");

            var isFork = (node.Nodes.Count > 1);
            var i = 1;
            foreach (var n in node.Nodes)
                DumpGraph(sb, isFork ? $"{prefix}.{i++}" : prefix, n);
        }

        private Node? TraceLine(int x, int y, char c, TraceDirection direction)
        {
            _buffer.SetTaken(x, y);

            if (direction.HasFlag(TraceDirection.Horizontal)) return TraceHorizontal(x, y, c, direction.HasFlag(TraceDirection.Right));
            if (direction.HasFlag(TraceDirection.Vertical)) return TraceVertical(x, y, c, direction.HasFlag(TraceDirection.Down));
            if (direction.HasFlag(TraceDirection.Diagonal)) return TraceDiagonal(x, y, c, direction.HasFlag(TraceDirection.Right), direction.HasFlag(TraceDirection.Down));
            return null;

        }

        private Node? TraceHorizontal(int x, int y, char c, bool right)
        {
            // step
            x = right ? x + 1 : x - 1;

            if (_buffer.IsTaken(x, y)) return null;

            var next = _buffer[x, y];

            // Node, soft bend
            if (next == '*' || next == '+' || next == '.' || next == '\'')
            {
                _buffer.SetTaken(x, y);
                return new Node(x, y, next);
            }

            if (next == '-')
            {
                _buffer.SetTaken(x, y);

                //if (c != next)
                //    return new Node(x, y, next);

                // Fork
                if (LinesFrom(x, y, next).Count() > 1)
                    return new Node(x, y, next);

                // keep going
                return TraceHorizontal(x, y, next, right) ?? new Node(x, y, next);
            }

            return null;
        }
        private Node? TraceVertical(int x, int y, char c, bool down)
        {
            // step
            y = down ? y + 1 : y - 1;

            if (_buffer.IsTaken(x, y)) return null;

            var next = _buffer[x, y];

            // Node, bend
            if (next == '*' || next == '+' || next == '/' || next == '\\' || next == '\'' ||
                (down && next == '\'') || (!down && next == '.'))
            {
                _buffer.SetTaken(x, y);
                return new Node(x, y, next);
            }

            if (next == '|')
            {
                _buffer.SetTaken(x, y);

                //if (c != next)
                //    return new Node(x, y, next);

                // Fork
                if (LinesFrom(x, y, next).Count() > 1)
                    return new Node(x, y, next);

                // keep going
                return TraceVertical(x, y, next, down) ?? new Node(x, y, next);
            }

            return null;
        }

        private Node? TraceDiagonal(int x, int y, char c, bool right, bool down)
        {
            // step
            x = right ? x + 1 : x - 1;
            y = down ? y + 1 : y - 1;

            if (_buffer.IsTaken(x, y)) return null;

            var next = _buffer[x, y];

            // Node, bend
            if (next == '*' || next == '+' ||
                (down && next == '\'') || (!down && next == '.'))
            {
                _buffer.SetTaken(x, y);
                return new Node(x, y, next);
            }

            if ((down == right && next == '\\') || (down != right && next == '/'))
            {
                _buffer.SetTaken(x, y);

                //if (c != next)
                //    return new Node(x, y, next);

                // Fork
                if (LinesFrom(x, y, next).Count() > 1)
                    return new Node(x, y, next);

                // keep going
                return TraceDiagonal(x, y, next, right, down) ?? new Node(x, y, next);
            }

            return null;
        }

        private IEnumerable<TraceDirection> LinesFrom(int x, int y, char c)
        {
            if (!_buffer.IsTaken(x, y - 1) && _buffer[x, y - 1] == '|')
                yield return TraceDirection.Vertical | TraceDirection.Up;

            if (!_buffer.IsTaken(x + 1, y - 1) && _buffer[x + 1, y - 1] == '/')
                yield return TraceDirection.Diagonal | TraceDirection.Right | TraceDirection.Up;

            if (!_buffer.IsTaken(x + 1, y) && _buffer[x + 1, y] == '-')
                yield return TraceDirection.Horizontal | TraceDirection.Right;

            if (!_buffer.IsTaken(x + 1, y + 1) && _buffer[x + 1, y + 1] == '\\')
                yield return TraceDirection.Diagonal | TraceDirection.Right | TraceDirection.Down;

            if (!_buffer.IsTaken(x, y + 1) && _buffer[x, y + 1] == '|')
                yield return TraceDirection.Vertical | TraceDirection.Down;

            if (!_buffer.IsTaken(x - 1, y + 1) && _buffer[x - 1, y + 1] == '/')
                yield return TraceDirection.Diagonal | TraceDirection.Left | TraceDirection.Down;

            if (!_buffer.IsTaken(x - 1, y) && _buffer[x - 1, y] == '-')
                yield return TraceDirection.Horizontal | TraceDirection.Left;

            if (!_buffer.IsTaken(x - 1, y - 1) && _buffer[x - 1, y - 1] == '\\')
                yield return TraceDirection.Diagonal | TraceDirection.Left | TraceDirection.Up;

            /*
            -1,-1  0,-1  1,-1
            -1, 0  0, 0  1, 0
            -1, 1  0, 1  1, 1
            */
        }
    }
}

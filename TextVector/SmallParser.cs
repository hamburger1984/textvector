using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextVector
{
    public class SmallParser
    {
        private readonly TextBuffer _buffer;

        public SmallParser(string[] lines)
        {
            _buffer = new TextBuffer(lines, neighborhood: 2);
        }

        public string Parse()
        {
            var result = new StringBuilder();

            for (var y = 0; y < _buffer.Height; y++)
                for (var x = 0; x < _buffer.Width; x++)
                {
                    var c = _buffer[x, y];

                    if (c == '-' && _buffer[x + 1, y] == '-')
                    {
                        var line = TraceLine(x, y, c, TraceDirection.Horizontal | TraceDirection.Right);
                        DumpLine(result, line);
                    }

                    if (c == '|' && _buffer[x, y + 1] == '|')
                    {
                        var line = TraceLine(x, y, c, TraceDirection.Vertical | TraceDirection.Down);
                        DumpLine(result, line);
                    }

                    if (c == '*' || c == '+')
                    {
                        if (_buffer[x, y - 1] == '|')
                        {
                            var line = TraceLine(x, y, c, TraceDirection.Vertical | TraceDirection.Up);
                            DumpLine(result, line);
                        }

                        if (_buffer[x + 1, y - 1] == '/')
                        {
                            var line = TraceLine(x, y, c, TraceDirection.Diagonal | TraceDirection.Right | TraceDirection.Up);
                            DumpLine(result, line);
                        }

                        if (_buffer[x + 1, y] == '-')
                        {
                            var line = TraceLine(x, y, c, TraceDirection.Horizontal | TraceDirection.Right);
                            DumpLine(result, line);
                        }

                        if (_buffer[x + 1, y + 1] == '\\')
                        {
                            var line = TraceLine(x, y, c, TraceDirection.Diagonal | TraceDirection.Right | TraceDirection.Down);
                            DumpLine(result, line);
                        }

                        if (_buffer[x, y + 1] == '|')
                        {
                            var line = TraceLine(x, y, c, TraceDirection.Vertical | TraceDirection.Down);
                            DumpLine(result, line);
                        }
                    }


                }

            return result.ToString();
        }

        private void DumpLine(StringBuilder sb, IReadOnlyList<Tuple<int, int, char>> points)
        {
            var first = points.First();
            sb.Append($"line ({first.Item1},{first.Item2},{first.Item3})");
            foreach (var point in points.Skip(1))
            {
                sb.Append($" to ({point.Item1},{point.Item2},{point.Item3})");
            }

            sb.AppendLine();
        }

        private List<Tuple<int, int, char>> TraceLine(int x, int y, char c, TraceDirection traceDirection)
        {
            var result = new List<Tuple<int, int, char>>();
            _buffer.SetTaken(x, y);

            Tuple<int, int, char>? next = null;
            if (traceDirection == (TraceDirection.Horizontal | TraceDirection.Right))
                next = TraceRight(x + 1, y, _buffer[x + 1, y]);
            if (traceDirection == (TraceDirection.Horizontal | TraceDirection.Left))
                next = TraceLeft(x - 1, y, _buffer[x - 1, y]);

            if (traceDirection == (TraceDirection.Vertical | TraceDirection.Down))
                next = TraceDown(x, y + 1, _buffer[x, y + 1]);
            if (traceDirection == (TraceDirection.Vertical | TraceDirection.Up))
                next = TraceUp(x, y - 1, _buffer[x, y - 1]);

            if (traceDirection == (TraceDirection.Diagonal | TraceDirection.Right | TraceDirection.Down))
                next = TraceDiagRD(x + 1, y + 1, _buffer[x + 1, y + 1]);
            if (traceDirection == (TraceDirection.Diagonal | TraceDirection.Left | TraceDirection.Down))
                next = TraceDiagLD(x - 1, y + 1, _buffer[x - 1, y + 1]);


            if (next != null)
            {
                result.Add(new Tuple<int, int, char>(x, y, c));
                result.Add(next);

                var directions = LinesFrom(next.Item1, next.Item2, next.Item3).ToList();
                foreach (var nextDirection in directions)
                {
                    result.AddRange(TraceLine(next.Item1, next.Item2, next.Item3, nextDirection));
                }
            }

            return result;
        }

        private IEnumerable<TraceDirection> LinesFrom(int x, int y, char c)
        {
            if (_buffer[x, y - 1] == '|') yield return TraceDirection.Vertical | TraceDirection.Up;
            if (_buffer[x + 1, y - 1] == '/') yield return TraceDirection.Diagonal | TraceDirection.Right | TraceDirection.Up;
            if (_buffer[x + 1, y] == '-') yield return TraceDirection.Horizontal | TraceDirection.Right;
            if (_buffer[x + 1, y + 1] == '\\') yield return TraceDirection.Diagonal | TraceDirection.Right | TraceDirection.Down;
            if (_buffer[x, y + 1] == '|') yield return TraceDirection.Vertical | TraceDirection.Down;
            if (_buffer[x - 1, y + 1] == '/') yield return TraceDirection.Diagonal | TraceDirection.Left | TraceDirection.Down;
            if (_buffer[x - 1, y] == '-') yield return TraceDirection.Horizontal | TraceDirection.Left;
            if (_buffer[x - 1, y - 1] == '\\') yield return TraceDirection.Diagonal | TraceDirection.Left | TraceDirection.Up;

            /*
            -1,-1  0,-1  1,-1
            -1, 0  0, 0  1, 0
            -1, 1  0, 1  1, 1
            */
        }


        public Tuple<int, int, char> TraceRight(int x, int y, char c)
        {
            // Node, soft bend
            if (c == '*' || c == '+' || c == '.' || c == '\'')
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }

            // Fork right
            if (c == '-' &&
                (_buffer[x, y - 1] == '|' ||
                _buffer[x + 1, y - 1] == '/' ||
                _buffer[x + 1, y + 1] == '\\' ||
                _buffer[x, y + 1] == '|'))
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }

            var next = _buffer[x + 1, y];

            if (next == '-' || next == '*' || next == '+' || next == '.' || next == '\'')
            {
                _buffer.SetTaken(x, y);
                return TraceRight(x + 1, y, next);
            }
            else
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }
        }

        public Tuple<int, int, char> TraceLeft(int x, int y, char c)
        {
            // Node, soft bend
            if (c == '*' || c == '+' || c == '.' || c == '\'')
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }

            // Fork left
            if (c == '-' &&
                (_buffer[x, y - 1] == '|' ||
                _buffer[x - 1, y - 1] == '\\' ||
                _buffer[x - 1, y + 1] == '/' ||
                _buffer[x, y + 1] == '|'))
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }

            var next = _buffer[x - 1, y];

            if (next == '-' || next == '*' || next == '+' || next == '.' || next == '\'')
            {
                _buffer.SetTaken(x, y);
                return TraceLeft(x - 1, y, next);
            }
            else
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }
        }


        public Tuple<int, int, char> TraceDown(int x, int y, char c)
        {
            // Node, bend
            if (c == '*' || c == '+' || c == '/' || c == '\\' || c == '\'')
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }

            // Fork
            if (c == '|' &&
                (_buffer[x - 1, y] == '-' ||
                _buffer[x - 1, y] == '/' ||
                _buffer[x + 1, y] == '-' ||
                _buffer[x + 1, y] == '\\'))
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }

            var next = _buffer[x, y + 1];

            if (next == '|' || next == '*' || next == '+' || next == '/' || next == '\\' || next == '\'')
            {
                _buffer.SetTaken(x, y);
                return TraceDown(x, y + 1, next);
            }

            _buffer.SetTaken(x, y);
            return new Tuple<int, int, char>(x, y, c);
        }

        public Tuple<int, int, char> TraceUp(int x, int y, char c)
        {
            // Node, bend
            if (c == '*' || c == '+' || c == '/' || c == '\\' || c == '.')
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }

            // Fork
            if (c == '|' &&
                (_buffer[x - 1, y] == '-' ||
                _buffer[x - 1, y] == '\\' ||
                _buffer[x + 1, y] == '/' ||
                _buffer[x + 1, y] == '-'))
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }

            var next = _buffer[x, y - 1];

            if (next == '|' || next == '*' || next == '+' || next == '/' || next == '\\' || next == '.')
            {
                _buffer.SetTaken(x, y);
                return TraceUp(x, y - 1, next);
            }

            _buffer.SetTaken(x, y);
            return new Tuple<int, int, char>(x, y, c);
        }

        private Tuple<int, int, char> TraceDiagRD(int x, int y, char c)
        {
            // Node, bend
            if (c == '*' || c == '+' || c == '|' || c == '-' || c == '\'')
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }

            // Fork
            if (c == '\\' &&
                (_buffer[x - 1, y] == '-' ||
                _buffer[x - 1, y] == '/' ||
                _buffer[x + 1, y] == '-'))
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }

            var next = _buffer.IsTaken(x, y) ? '\0' : _buffer[x + 1, y + 1];

            if (next == '\\' || next == '*' || next == '+' || next == '|' || next == '-' || next == '\'')
            {
                _buffer.SetTaken(x, y);
                return TraceDiagRD(x + 1, y + 1, next);
            }

            _buffer.SetTaken(x, y);
            return new Tuple<int, int, char>(x, y, c);
        }
        private Tuple<int, int, char> TraceDiagLD(int x, int y, char c)
        {
            // Node, bend
            if (c == '*' || c == '+' || c == '|' || c == '-' || c == '\'')
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }

            // Fork
            if (c == '\\' &&
                (_buffer[x - 1, y] == '-' ||
                _buffer[x - 1, y] == '\\' ||
                _buffer[x + 1, y] == '-'))
            {
                _buffer.SetTaken(x, y);
                return new Tuple<int, int, char>(x, y, c);
            }

            var next = _buffer[x - 1, y + 1];

            if (next == '/' || next == '*' || next == '+' || next == '|' || next == '-' || next == '\'')
            {
                _buffer.SetTaken(x, y);
                return TraceDiagLD(x - 1, y + 1, next);
            }

            _buffer.SetTaken(x, y);
            return new Tuple<int, int, char>(x, y, c);
        }


    }
}

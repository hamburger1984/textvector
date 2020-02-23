using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                        var line = TraceLine(x, y, c, Direction.Horizontal | Direction.Right);
                        DumpLine(result, line);
                    }

                    if (c == '|' && _buffer[x, y + 1] == '|')
                    {
                        var line = TraceLine(x, y, c, Direction.Vertical | Direction.Down);
                        DumpLine(result, line);
                    }

                    if (c == '*' || c == '+')
                    {
                        if (_buffer[x, y - 1] == '|')
                        {
                            var line = TraceLine(x, y, c, Direction.Vertical | Direction.Up);
                            DumpLine(result, line);
                        }

                        if (_buffer[x + 1, y - 1] == '/')
                        {
                            var line = TraceLine(x, y, c, Direction.Diagonal | Direction.Right | Direction.Up);
                            DumpLine(result, line);
                        }

                        if (_buffer[x + 1, y] == '-')
                        {
                            var line = TraceLine(x, y, c, Direction.Horizontal | Direction.Right);
                            DumpLine(result, line);
                        }

                        if (_buffer[x + 1, y + 1] == '\\')
                        {
                            var line = TraceLine(x, y, c, Direction.Diagonal | Direction.Right | Direction.Down);
                            DumpLine(result, line);
                        }

                        if (_buffer[x, y + 1] == '|')
                        {
                            var line = TraceLine(x, y, c, Direction.Vertical | Direction.Down);
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

        public List<Tuple<int, int, char>> TraceLine(int x, int y, char c, Direction direction)
        {
            var result = new List<Tuple<int, int, char>>();
            _buffer[x, y] = ' ';

            Tuple<int, int, char>? next = null;
            if (direction == (Direction.Horizontal | Direction.Right))
                next = TraceRight(x + 1, y, _buffer[x + 1, y]);
            if (direction == (Direction.Horizontal | Direction.Left))
                next = TraceLeft(x - 1, y, _buffer[x - 1, y]);
            if (direction == (Direction.Vertical | Direction.Down))
                next = TraceDown(x, y + 1, _buffer[x, y + 1]);
            if (direction == (Direction.Vertical | Direction.Up))
                next = TraceUp(x, y - 1, _buffer[x, y - 1]);

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

        private IEnumerable<Direction> LinesFrom(int x, int y, char c)
        {
            if (_buffer[x, y - 1] == '|') yield return Direction.Vertical | Direction.Up;
            if (_buffer[x + 1, y - 1] == '/') yield return Direction.Diagonal | Direction.Right | Direction.Up;
            if (_buffer[x + 1, y] == '-') yield return Direction.Horizontal | Direction.Right;
            if (_buffer[x + 1, y + 1] == '\\') yield return Direction.Diagonal | Direction.Right | Direction.Down;
            if (_buffer[x, y + 1] == '|') yield return Direction.Vertical | Direction.Down;
            if (_buffer[x - 1, y + 1] == '/') yield return Direction.Diagonal | Direction.Left | Direction.Down;
            if (_buffer[x - 1, y] == '-') yield return Direction.Horizontal | Direction.Left;
            if (_buffer[x - 1, y - 1] == '\\') yield return Direction.Diagonal | Direction.Left | Direction.Up;

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
                _buffer[x, y] = ' ';
                return new Tuple<int, int, char>(x, y, c);
            }

            // Fork right
            if (c == '-' &&
                (_buffer[x, y - 1] == '|' ||
                _buffer[x + 1, y - 1] == '/' ||
                _buffer[x + 1, y + 1] == '\\' ||
                _buffer[x, y + 1] == '|'))
            {
                _buffer[x, y] = ' ';
                return new Tuple<int, int, char>(x, y, c);
            }

            var next = _buffer[x + 1, y];

            if (next == '-' || next == '*' || next == '+' || next == '.' || next == '\'')
            {
                _buffer[x, y] = ' ';
                return TraceRight(x + 1, y, next);
            }
            else
            {
                _buffer[x, y] = ' ';
                return new Tuple<int, int, char>(x, y, c);
            }
        }

        public Tuple<int, int, char> TraceLeft(int x, int y, char c)
        {
            // Node, soft bend
            if (c == '*' || c == '+' || c == '.' || c == '\'')
            {
                _buffer[x, y] = ' ';
                return new Tuple<int, int, char>(x, y, c);
            }

            // Fork left
            if (c == '-' &&
                (_buffer[x, y - 1] == '|' ||
                _buffer[x - 1, y - 1] == '\\' ||
                _buffer[x - 1, y + 1] == '/' ||
                _buffer[x, y + 1] == '|'))
            {
                _buffer[x, y] = ' ';
                return new Tuple<int, int, char>(x, y, c);
            }

            var next = _buffer[x - 1, y];

            if (next == '-' || next == '*' || next == '+' || next == '.' || next == '\'')
            {
                _buffer[x, y] = ' ';
                return TraceLeft(x - 1, y, next);
            }
            else
            {
                _buffer[x, y] = ' ';
                return new Tuple<int, int, char>(x, y, c);
            }
        }


        public Tuple<int, int, char> TraceDown(int x, int y, char c)
        {
            // Node, bend
            if (c == '*' || c == '+' || c == '/' || c == '\\')
            {
                _buffer[x, y] = ' ';
                return new Tuple<int, int, char>(x, y, c);
            }

            // Fork
            if (c == '|' &&
                (_buffer[x - 1, y] == '-' ||
                _buffer[x - 1, y] == '/' ||
                _buffer[x + 1, y] == '-' ||
                _buffer[x + 1, y] == '\\'))
            {
                _buffer[x, y] = ' ';
                return new Tuple<int, int, char>(x, y, c);
            }

            var next = _buffer[x, y + 1];

            if (next == '|' || next == '*' || next == '+' || next == '/' || next == '\\')
            {
                _buffer[x, y] = ' ';
                return TraceDown(x, y + 1, next);
            }

            _buffer[x, y] = ' ';
            return new Tuple<int, int, char>(x, y, c);
        }

        public Tuple<int, int, char> TraceUp(int x, int y, char c)
        {
            // Node, bend
            if (c == '*' || c == '+' || c == '/' || c == '\\')
            {
                _buffer[x, y] = ' ';
                return new Tuple<int, int, char>(x, y, c);
            }

            // Fork
            if (c == '|' &&
                (_buffer[x - 1, y] == '-' ||
                _buffer[x - 1, y] == '\\' ||
                _buffer[x + 1, y] == '/' ||
                _buffer[x + 1, y] == '-'))
            {
                _buffer[x, y] = ' ';
                return new Tuple<int, int, char>(x, y, c);
            }

            var next = _buffer[x, y - 1];

            if (next == '|' || next == '*' || next == '+' || next == '/' || next == '\\')
            {
                _buffer[x, y] = ' ';
                return TraceUp(x, y - 1, next);
            }

            _buffer[x, y] = ' ';
            return new Tuple<int, int, char>(x, y, c);
        }


        [Flags]
        public enum Direction
        {
            Horizontal = 0b1,
            Vertical = 0b10,
            Diagonal = 0b100,
            Left = 0b1000,
            Right = 0b10000,
            Up = 0b100000,
            Down = 0b1000000
        }
    }
}

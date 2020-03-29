using System;
using System.Collections.Generic;
using System.Linq;

namespace TextVector.Buffer
{
    public class TextBuffer
    {
        private readonly char[] _buffer;
        private readonly int _lineWidth;
        private const int Neighborhood = 1;
        private readonly int[] _figures;
        private int lastId = 0;

        public TextBuffer(IReadOnlyList<string> lines)
        {
            Height = lines.Count;
            Width = lines.Select(l => l.Length).Max();
            _lineWidth = Width + 2 * Neighborhood;

            _buffer = new char[(Height + 2 * Neighborhood) * _lineWidth];
            _figures = new int[(Height + 2 * Neighborhood) * _lineWidth];

            //Array.Fill(_buffer, '\0', 0, Neighborhood * _lineWidth);
            //Array.Fill(_buffer, '\0', (Height + Neighborhood) * _lineWidth, Neighborhood * _lineWidth);

            Array.Fill(_figures, int.MaxValue, 0, Neighborhood * _lineWidth);
            Array.Fill(_figures, int.MaxValue, (Height + Neighborhood) * _lineWidth, Neighborhood * _lineWidth);

            for (var y = 0; y < Height; y++)
            {
                //Array.Fill(_buffer, '\0', (y + Neighborhood) * _lineWidth, Neighborhood);
                //Array.Fill(_buffer, '\0', (y + 1 + Neighborhood) * _lineWidth - Neighborhood, Neighborhood);
                Array.Fill(_figures, int.MaxValue, (y + Neighborhood) * _lineWidth, Neighborhood);
                Array.Fill(_figures, int.MaxValue, (y + 1 + Neighborhood) * _lineWidth - Neighborhood, Neighborhood);
                var lineLength = lines[y].Length;
                Array.Copy(lines[y].ToCharArray(), 0, _buffer, (y + Neighborhood) * _lineWidth + Neighborhood,
                    lineLength);
                if (lineLength < Width)
                    Array.Fill(_buffer, ' ', (y + Neighborhood) * _lineWidth + Neighborhood + lineLength,
                        Width - lineLength);
            }
        }

        public int Height { get; }
        public int Width { get; }

        public char this[int x, int y] => _buffer[ToIndex(x, y)];

        private int ToIndex(int x, int y)
        {
            return (Neighborhood + y) * _lineWidth + Neighborhood + x;
        }

        private const int TextPremarked = int.MaxValue - 1;

        public IEnumerable<(int x, char c, bool isTaken)> GetLine(int y)
        {

            if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException(nameof(y));

            var start = ToIndex(0, y);
            for (var x = 0; x < Width; x++)
            {
                var figureId = _figures[start + x];
                yield return (x, _buffer[start + x], figureId != 0 && figureId != TextPremarked);
            }
        }


        public int IsFigure(int x, int y)
        {
            return _figures[ToIndex(x, y)];
        }

        public int PeekNextId()
        {
            return lastId + 1;
        }

        public int UseNextId()
        {
            return ++lastId;
        }

        public void PreMarkText(int x, int y)
        {
            _figures[ToIndex(x, y)] = TextPremarked;
        }

        public void SetFigure(int x, int y, int figureId)
        {
            _figures[ToIndex(x, y)] = figureId;
        }
    }
}
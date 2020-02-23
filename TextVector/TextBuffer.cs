﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TextVector
{
    public class TextBuffer
    {
        public TextBuffer(IReadOnlyList<string> lines, int neighborhood = 2)
        {
            Height = lines.Count;
            Width = lines.Select(l => l.Length).Max();
            _neighborhood = neighborhood;
            _lineWidth = Width + 2 * neighborhood;

            _buffer = new char[(Height + 2 * neighborhood) * _lineWidth];

            if (neighborhood > 0)
            {
                Array.Fill(_buffer, '\0', 0, neighborhood * _lineWidth);
                Array.Fill(_buffer, '\0', (Height + neighborhood) * _lineWidth, neighborhood * _lineWidth);
            }

            for (var y = 0; y < Height; y++)
            {
                Array.Fill(_buffer, '\0', (y + neighborhood) * _lineWidth, neighborhood);
                Array.Fill(_buffer, '\0', (y + 1 + neighborhood) * _lineWidth - neighborhood, neighborhood);
                var lineLength = lines[y].Length;
                Array.Copy(lines[y].ToCharArray(), 0, _buffer, (y + neighborhood) * _lineWidth + neighborhood, lineLength);
                if (lineLength < Width)
                {
                    Array.Fill(_buffer, ' ', (y + neighborhood) * _lineWidth + neighborhood + lineLength, Width - lineLength);
                }
            }
        }

        private readonly char[] _buffer;
        private readonly int _neighborhood;
        private readonly int _lineWidth;

        public int Height { get; }
        public int Width { get; }

        public char this[int x, int y]
        {
            get => _buffer[(_neighborhood + y) * _lineWidth + _neighborhood + x];
            set => _buffer[(_neighborhood + y) * _lineWidth + _neighborhood + x] = value;
        }

        public char[] Neighborhood(int x, int y, int distance = 1)
        {
            Debug.Assert(distance <= _neighborhood);

            var result = new char[(distance * 2 + 1) * (distance * 2 + 1)];
            var index = 0;

            for (var row = y - distance; row <= y + distance; row++)
                for (var column = x - distance; column <= x + distance; column++)
                    result[index++] = this[column, row];


            return result;
        }

    }
}
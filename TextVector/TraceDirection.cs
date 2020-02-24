using System;

namespace TextVector
{
    [Flags]
    public enum TraceDirection
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
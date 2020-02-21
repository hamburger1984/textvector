using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace TextVector
{
    public class Parser
    {
        private const int charH = 8;
        private const int charW = 6;

        public IEnumerable<Parsed> Parse(string input)
        {
            var result = new List<Parsed>();

            var reader = new StringReader(input);
            var x = 0; var y = -1;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                x = 0;
                y++;
                var span = line.AsSpan();
                if (span.IsWhiteSpace())
                    continue;


                while (x < span.Length)
                {
                    switch (span[x])
                    {
                        case var m when char.IsWhiteSpace(span[x]):
                            var spaces = CountWhere(span, x, char.IsWhiteSpace);
                            x += spaces;
                            break;
                        case '*':
                        case '_':
                            result.Add(Parsed.Text(x, y, span[x].ToString()));
                            x++;
                            break;
                        case '-':
                            var dashes = CountWhere(span, x, c => c == '-');
                            if (dashes > 1)
                            {
                                result.Add(Parsed.Line(x, y, x + dashes, y));
                            }
                            else
                                result.Add(Parsed.Text(x, y, "-"));

                            x += dashes;
                            break;
                        case var m when char.IsLetterOrDigit(span[x]):
                            x += Text(span: span, x: x, y: y, result);
                            break;
                        default:
                            result.Add(new Parsed(x, y, value: span[x].ToString()));
                            x++;
                            break;

                    }
                }
            }

            return result;
        }


        private int Text(in ReadOnlySpan<char> span, in int x, in int y, IList<Parsed> result)
        {
            var letters = CountWhere(span, x, char.IsLetterOrDigit);


            result.Add(Parsed.Text(x, y, span.Slice(x, letters).ToString()));
            return letters;
        }

        private int CountWhere(in ReadOnlySpan<char> value, in int start, Func<char, bool> condition)
        {

            var end = start;
            var length = value.Length;
            while (end < length && condition(value[end]))
            {
                end++;
            }

            return end - start;
        }
    }

    public enum Element
    {
        Line,
        Text,
        Unknown
    }

    public readonly struct Parsed
    {
        public int X { get; }
        public int Y { get; }
        public int? X2 { get; }
        public int? Y2 { get; }
        public Element Element { get; }
        public string? Value { get; }
        public static Parsed Line(int x, int y, int x2, int y2) { return new Parsed(x, y, x2: x2, y2: y2, element: Element.Line); }
        public static Parsed Text(int x, int y, string value) { return new Parsed(x, y, value: value, element: Element.Text); }
        public Parsed(int x, int y, Element element = Element.Unknown, string? value = null, int? x2 = null, int? y2 = null)
        {
            X = x;
            Y = y;
            X2 = x2;
            Y2 = y2;
            Element = element;
            Value = value;
        }
    }
}

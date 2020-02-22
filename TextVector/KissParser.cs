using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace TextVector
{
    public class KissParser
    {
        public KissParser() { }

        public IEnumerable<ImageEntity> Parse(IEnumerable<string> lines, Action<string>? outputWriter = null)
        {
            var description = new Dictionary<int, Dictionary<int, string>>();
            var width = 0;
            var height = 0;

            var y = 0;

            foreach (var line in lines)
            {
                var trimmed = line.TrimEnd(' ', '\t', '\n', '\r');
                width = Math.Max(trimmed.Length, width);
                height++;

                description[y] = DescribeLine(trimmed);
                y++;

            }
            outputWriter?.Invoke($"Text is {width} x {height}.");

            for (y = 0; y < height; y++)
            {
                var line = new StringBuilder();
                var d = description[y];

                for (var x = 0; x < width; x++)
                {
                    line.AppendFormat(" {0,4}", d.ContainsKey(x) ? d[x] : "");
                }
                outputWriter?.Invoke(line.ToString());
            }


            //var y = 0;
            //foreach (var line in lines)
            //{
            //    var lastLine = y > 0 ? description[y - 1] : null;

            //}
            return Enumerable.Empty<ImageEntity>();

        }

        private Dictionary<int, string> DescribeLine(string line)
        {
            var lineDescription = new Dictionary<int, string>();
            var chars = line.ToCharArray();

            for (var x = 0; x < chars.Length; x++)
            {
                switch (chars[x])
                {
                    case var _ when char.IsWhiteSpace(chars[x]):
                        continue;
                    case '-':
                        lineDescription[x] = "l:h";
                        break;
                    case '|':
                        lineDescription[x] = "l:v";
                        break;
                    case '(':
                        lineDescription[x] = "c:l";
                        break;
                    case ')':
                        lineDescription[x] = "c:r";
                        break;
                    case '*':
                        lineDescription[x] = "as";
                        break;
                    case '+':
                        lineDescription[x] = "l:x";
                        break;
                    case '^':
                        lineDescription[x] = "ar:u";
                        break;
                    case '<':
                        lineDescription[x] = "ar:l";
                        break;
                    case '>':
                        lineDescription[x] = "ar:r";
                        break;
                    case '.':
                        lineDescription[x] = "dt";
                        break;
                    case '`':
                        lineDescription[x] = "q:d";
                        break;
                    case '´':
                        lineDescription[x] = "q:u";
                        break;
                    case '\'':
                        lineDescription[x] = "q:s";
                        break;
                    case '"':
                        lineDescription[x] = "q:q";
                        break;
                    case '/':
                        lineDescription[x] = "l:u";
                        break;
                    case '\\':
                        lineDescription[x] = "l:d";
                        break;
                    case var ld when char.IsLetterOrDigit(chars[x]):
                        lineDescription[x] = $"t:{ld}";
                        break;
                    case var p when char.IsPunctuation(chars[x]):
                        lineDescription[x] = $"p:{p}";
                        break;
                    default:
                        // NOP
                        break;
                }
            }

            return lineDescription;
        }
    }



    public readonly struct ImageEntity
    {

        public EntityKind Kind { get; }
    }

    public enum EntityKind
    {
    }
}

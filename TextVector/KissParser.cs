using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TextVector
{
    public class KissParser
    {
        private readonly Dictionary<int, Dictionary<int, string>> description = new Dictionary<int, Dictionary<int, string>>();
        private readonly Dictionary<int, Dictionary<int, ImageEntity>> entities = new Dictionary<int, Dictionary<int, ImageEntity>>();
        private int width = 0;
        private int height = 0;
        private int cellWidth = 8;
        private int cellHeight = 16;

        public KissParser() { }

        public IEnumerable<ImageEntity> Parse(IEnumerable<string> lines, Action<string>? outputWriter = null)
        {
            description.Clear();
            entities.Clear();

            width = 0;
            height = 0;

            foreach (var line in lines)
            {
                var trimmed = line.TrimEnd(' ', '\t', '\n', '\r');
                width = Math.Max(trimmed.Length, width);
                DescribeLine(trimmed, height);
                height++;
            }

            if (outputWriter != null)
            {
                outputWriter($"Text is {width} x {height}.");
                DumpDescription(outputWriter);
            }


            return entities.SelectMany(l => l.Value).Select(p => p.Value);
        }

        private void DumpDescription(Action<string> outputWriter)
        {
            for (var y = 0; y < height; y++)
            {
                var line = new StringBuilder();
                var d = description[y];

                for (var x = 0; x < width; x++)
                {
                    line.AppendFormat(" {0,4}", d.ContainsKey(x) ? d[x] : "");
                }

                outputWriter.Invoke(line.ToString());
            }
        }

        private void DescribeLine(string line, int y)
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

            description[y] = lineDescription;
        }


    }


    public readonly struct ImageEntity
    {
        public static ImageEntity Text(int x, int y, string content, bool bold = false, bool emph = false)
        {
            return new ImageEntity(EntityKind.Text, x, y, content, "text" + (bold ? " .bold" : "") + (emph ? " .emph" : ""));
        }

        public ImageEntity(EntityKind kind, int x, int y, string? content = null, string? classes = null)
        {
            Kind = kind;
            X = x;
            Y = y;
            Classes = classes;
            Content = content;
        }

        public EntityKind Kind { get; }
        public int X { get; }
        public int Y { get; }
        public string? Classes { get; }
        public string? Content { get; }
    }

    public enum EntityKind
    {
        Text
    }
}

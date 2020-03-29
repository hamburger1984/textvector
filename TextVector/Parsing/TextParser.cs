using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextVector.Buffer;

namespace TextVector.Parsing
{
    public class TextParser
    {
        private readonly TextBuffer _buffer;
        private readonly char[] TextStartChars = "\"'([{".ToCharArray();

        public TextParser(TextBuffer buffer)
        {
            _buffer = buffer;
        }

        public IEnumerable<Text> Parse()
        {
            var currentText = new StringBuilder();
            for (var y = 0; y < _buffer.Height; y++)
            {
                using var lineFigures = _buffer.GetLine(y).GetEnumerator();

                var x = 0;
                var last = (word: 0, white: 0, punct: 0);

                while (lineFigures.MoveNext())
                {
                    char c;
                    bool taken;
                    (x, c, taken) = lineFigures.Current;
                    // Console.WriteLine($"{x}[{c}] -> '{currentText.ToString()}' ({currentText.Length})");
                    if (taken)
                    {
                        if (currentText.Length > 0)
                        {
                            Console.WriteLine($"TAKEN {x}[{c}] -> '{currentText.ToString()}' ({currentText.Length})");
                            yield return CreateText(x, y, currentText);
                            last = (0, 0, 0);
                        }
                        continue;
                    }

                    if (char.IsLetterOrDigit(c))
                    {
                        currentText.Append(c);
                        _buffer.SetFigure(x, y, _buffer.PeekNextId());
                        last = (last.word + 1, 0, 0);
                        continue;
                    }
                    if (currentText.Length == 0)
                    {
                        if (TextStartChars.Contains(c))
                        {
                            currentText.Append(c);
                            _buffer.SetFigure(x, y, _buffer.PeekNextId());
                            last = (0, 0, 1);
                            continue;
                        }
                        continue;
                    }


                    if (char.IsWhiteSpace(c))
                    {
                        currentText.Append(c);
                        _buffer.SetFigure(x, y, _buffer.PeekNextId());
                        last = (0, last.white + 1, 0);
                        if (last.white > 1)
                        {
                            Console.WriteLine($"WHITE {x}[{c}] -> '{currentText.ToString()}' ({currentText.Length})");
                            yield return CreateText(x + 1, y, currentText);
                            last = (0, 0, 0);
                        }
                    }

                    if (char.IsPunctuation(c))
                    {
                        currentText.Append(c);
                        _buffer.SetFigure(x, y, _buffer.PeekNextId());
                        last = (0, 0, last.punct + 1);
                        if (last.punct > 1)
                        {
                            Console.WriteLine($"PUNCT {x}[{c}] -> '{currentText.ToString()}' ({currentText.Length})");
                            yield return CreateText(x + 1, y, currentText);
                            last = (0, 0, 0);
                        }
                    }
                }

                if (currentText.Length > 0)
                {
                    Console.WriteLine($"END {x} -> '{currentText.ToString()}' ({currentText.Length})");
                    yield return CreateText(x + 1, y, currentText);
                }
            }
            // foreach line
            // if cell is not part of figure, yet
            // collect word chars, tolerating single non-word chars
            // create new text entity
            // continue until all cells checked.
        }

        private Text CreateText(int x, int y, StringBuilder builder)
        {
            var text = new Text(x - builder.Length, y, _buffer.UseNextId(), builder.ToString().Trim());
            builder.Length = 0;
            return text;
        }
    }
}
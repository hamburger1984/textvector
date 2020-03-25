using System.Collections.Generic;
using System.Text;
using TextVector.Buffer;

namespace TextVector.Parsing
{
    public class TextParser
    {
        private readonly TextBuffer _buffer;

        public TextParser(TextBuffer buffer)
        {
            _buffer = buffer;
        }

        public IEnumerable<Text> Parse()
        {
            for (var y = 0; y < _buffer.Height; y++)
            {
                using var lineFigures = _buffer.GetLine(y).GetEnumerator();

                var currentText = new StringBuilder();

                while (lineFigures.MoveNext())
                {
                    var (x, c, taken) = lineFigures.Current;
                    if (taken)
                    {
                        if (currentText.Length > 0)
                        {
                            yield return new Text(x - currentText.Length, y, _buffer.NextFigureId(),
                                currentText.ToString());
                            currentText.Length = 0;
                        }
                        continue;
                    }

                    if (char.IsLetterOrDigit(c)) currentText.Append(c);
                    if (currentText.Length > 0 && (char.IsWhiteSpace(c) || char.IsPunctuation(c)))
                        currentText.Append(c);
                }
            }
            // foreach line
            // if cell is not part of figure, yet
            // collect word chars, tolerating single non-word chars
            // create new text entity
            // continue until all cells checked.
        }

    }
}
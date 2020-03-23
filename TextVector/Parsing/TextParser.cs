using System.Collections.Generic;
using System.Linq;
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
            // foreach line
            // if cell is not part of figure, yet
            // collect word chars, tolerating single non-word chars
            // create new text entity
            // continue until all cells checked.
            return Enumerable.Empty<Text>();
        }

    }
}
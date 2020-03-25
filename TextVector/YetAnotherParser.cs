using System.Collections.Generic;
using System.Linq;
using TextVector.Buffer;
using TextVector.Parsing;
using TextVector.Writing;

namespace TextVector
{
    public class YetAnotherParser
    {
        private readonly TextBuffer _buffer;

        public YetAnotherParser(IReadOnlyList<string> lines)
        {
            _buffer = new TextBuffer(lines);
        }

        public string ParseToText()
        {
            var writer = new TextDumper();
            return writer.WriteString(ParseFigures(), Enumerable.Empty<Text>());
        }

        public void ParseToSvg(string filename)
        {
            var writer = new SvgGenerator(_buffer.Width, _buffer.Height);
            writer.WriteFile(filename, ParseFigures(), Enumerable.Empty<Text>());
        }

        private IEnumerable<Figure> ParseFigures()
        {
            return new FigureParser(_buffer).Parse();
        }
    }
}
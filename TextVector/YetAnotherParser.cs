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
            var (figures, texts) = Parse();
            return writer.WriteString(figures, texts);
        }

        public void ParseToSvg(string filename)
        {
            var writer = new SvgGenerator(_buffer.Width, _buffer.Height);
            var (figures, texts) = Parse();
            writer.WriteFile(filename, figures, texts);
        }

        private (IEnumerable<Figure> figures, IEnumerable<Text> texts) Parse()
        {
            var textParser = new TextParser(_buffer);
            textParser.PreMarkText();
            var figures = new FigureParser(_buffer).Parse().ToList();
            var texts = textParser.Parse().ToList();
            return (figures, texts);
        }
    }
}
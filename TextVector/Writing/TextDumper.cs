using System.Collections.Generic;
using System.IO;
using System.Text;
using TextVector.Parsing;

namespace TextVector.Writing
{
    public class TextDumper : IWriter
    {
        private void DumpGraph(StringBuilder sb, Figure node, string suffix = "")
        {
            sb.AppendLine($"{node.FigureId}{suffix} ({node.X},{node.Y},{node.C})");

            var isFork = node.Nodes.Count > 1;
            var i = 1;
            foreach (var n in node.Nodes)
                DumpGraph(sb, n, isFork ? $"{suffix}.{i++}" : suffix);
        }

        public string WriteString(IEnumerable<Figure> figures, IEnumerable<Text> texts)
        {
            var result = new StringBuilder();
            foreach (var figure in figures) DumpGraph(result, figure);
            foreach (var text in texts) DumpText(result, text);
            return result.ToString();
        }

        private void DumpText(StringBuilder sb, Text text)
        {
            sb.AppendLine($"{text.TextId} ({text.X},{text.Y},{text.Value})");
        }

        public void WriteFile(string filename, IEnumerable<Figure> figures, IEnumerable<Text> texts)
        {
            var dump = WriteString(figures, texts);
            if (!string.IsNullOrEmpty(filename))
                File.WriteAllText(filename, dump);
        }
    }
}
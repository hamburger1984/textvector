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

        public string WriteString(IEnumerable<Figure> figures)
        {
            var result = new StringBuilder();
            foreach (var figure in figures) DumpGraph(result, figure);
            return result.ToString();
        }

        public void WriteFile(string filename, IEnumerable<Figure> figures)
        {
            var dump = WriteString(figures);
            if (!string.IsNullOrEmpty(filename))
                File.WriteAllText(filename, dump);
        }
    }
}
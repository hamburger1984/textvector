using System.Collections.Generic;
using TextVector.Parsing;

namespace TextVector.Writing
{
    public interface IWriter
    {
        string WriteString(IEnumerable<Figure> figures, IEnumerable<Text> texts);
        void WriteFile(string filename, IEnumerable<Figure> figures, IEnumerable<Text> texts);
    }
}
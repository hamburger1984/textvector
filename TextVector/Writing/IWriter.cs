using System.Collections.Generic;
using TextVector.Parsing;

namespace TextVector.Writing
{
    public interface IWriter
    {
        string WriteString(IEnumerable<Node> figures);
        void WriteFile(IEnumerable<Node> figures, string filename);
    }
}
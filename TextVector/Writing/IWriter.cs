using System.Collections.Generic;
using TextVector.Parsing;

namespace TextVector.Writing
{
    public interface IWriter
    {
        string WriteString(IEnumerable<Node> figures);
        void WriteFile(string filename, IEnumerable<Node> figures);
    }
}
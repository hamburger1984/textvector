using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace TextVector
{
    public class YetAnotherParser
    {
        private readonly TextBuffer _buffer;

        public YetAnotherParser(IReadOnlyList<string> lines)
        {
            _buffer = new TextBuffer(lines, neighborhood: 2);
        }

        private readonly char[] _lineChars = {
            '-', '|',
            '+', '*',
            '/', '\\',
            '.', '\''
        };

        public string ParseToText()
        {
            var result = new StringBuilder();
            foreach (var figure in ParseFigures())
            {
                DumpGraph(result, figure);
            }

            return result.ToString();
        }
        private void DumpGraph(StringBuilder sb, Node node, string suffix = "")
        {
            sb.AppendLine($"{node.FigureId}{suffix} ({node.X},{node.Y},{node.C})");

            var isFork = (node.Nodes.Count > 1);
            var i = 1;
            foreach (var n in node.Nodes)
                DumpGraph(sb, n, isFork ? $"{suffix}.{i++}" : suffix);
        }

        private const int SvgCellWidth = 4;
        private const int SvgCellHeight = 6;
        private const int SvgHalfCellWidth = 2;
        private const int SvgHalfCellHeight = 3;

        public string ParseToSvg()
        {

            var doc = new XDocument();
            XNamespace ns = ("http://www.w3.org/2000/svg");
            var svg = new XElement(ns + "svg",
                new XAttribute("width", $"{SvgCellWidth * _buffer.Width}"),
                new XAttribute("height", $"{SvgCellHeight * _buffer.Height}"));
            doc.Add(svg);

            var defs = new XElement(ns + "defs");
            BuildSvgDefs(defs, ns);
            svg.Add(defs);

            var style = new XElement(ns + "style",
                new XAttribute("type", "text/css"));
            var styleContent = new XCData(@"
line, path {
    fill: none;
    fill-opacity: 1;
    stroke: black;
    stoke-width: 2;
    stroke-linecap: round;
    stroke-linejoin: miter;
    stroke-opacity: 1;
}
.end-arrow{
    marker-end: url(#arrow);
}
.start-arrow{
    marker-start: url(#arrow);
}
.end-arrow-reverse{
    marker-end: url(#arrow-reverse);
}
.start-arrow-reverse{
    marker-start: url(#arrow-reverse);
}

");
            style.Add(styleContent);
            svg.Add(style);

            foreach (var figure in ParseFigures())
            {
                var figureGroup = new XElement(ns + "g",
                    new XAttribute("id", $"figure{figure.FigureId}"));
                NodeToSvg(figureGroup, ns, figure);
                svg.Add(figureGroup);

                //var pathData = GenerateSvgPathData(figure, SvgCellWidth, SvgCellHeight, true);

                //var path = new XElement(ns + "path",
                //        new XAttribute("id", $"path{figure.FigureId}"),
                //        //new XAttribute("class", ".stroke"),
                //        new XAttribute("d", pathData));
                //svg.Add(path);
            }

            return doc.ToString();
        }

        private void BuildSvgDefs(XElement defs, XNamespace ns)
        {
            defs.Add(new XElement(ns + "marker",
                new XAttribute("id", "arrow"),
                new XAttribute("markerHeight", "7"),
                new XAttribute("markerWidth", "7"),
                new XAttribute("orient", "auto-start-reverse"),
                new XAttribute("refX", "4"),
                new XAttribute("refY", "2"),
                new XAttribute("viewBox", "-2 -2 8 8"),
                new XElement(ns + "polygon",
                    new XAttribute("points", "0,0 0,4 4,2 0,0"))
                ),
                new XElement(ns + "marker",
                new XAttribute("id", "arrow-reverse"),
                new XAttribute("markerHeight", "7"),
                new XAttribute("markerWidth", "7"),
                new XAttribute("orient", "auto-start-reverse"),
                new XAttribute("refX", "4"),
                new XAttribute("refY", "2"),
                new XAttribute("viewBox", "-2 -2 8 8"),
                new XElement(ns + "polygon",
                    new XAttribute("points", "4,0 4,4 0,2 4,0"))
                ));
        }

        private void NodeToSvg(XElement el, XNamespace ns, Node node)
        {
            var startCap = SvgLineCap(node.C, true);
            foreach (var dest in node.Nodes)
            {
                var endCap = SvgLineCap(dest.C, false);
                var classes = string.Join(' ', startCap, endCap);

                el.Add(new XElement(ns + "line",
                    new XAttribute("class", classes),
                    new XAttribute("x1", SvgHalfCellWidth + node.X * SvgCellWidth),
                    new XAttribute("y1", SvgHalfCellHeight + node.Y * SvgCellHeight),
                    new XAttribute("x2", SvgHalfCellWidth + dest.X * SvgCellWidth),
                    new XAttribute("y2", SvgHalfCellHeight + dest.Y * SvgCellHeight)));

                NodeToSvg(el, ns, dest);
            }
        }

        private string? SvgLineCap(char c, bool start)
        {
            var prefix = start ? "start-" : "end-";

            if (c == 'o')
                return prefix + "round-filled";
            if (c == 'O')
                return prefix + "round-big";
            if (c == '*')
                return prefix + "round-big-filled";
            if (c == '>' || c == 'v')
                if (start)
                    return prefix + "arrow-reverse";
                else
                    return prefix + "arrow";
            if (c == '<' || c == '^')
                if (start)
                    return prefix + "arrow";
                else
                    return prefix + "arrow-reverse";

            return null;
        }

        private string GenerateSvgPathData(in Node node, int scaleX, int scaleY, bool isFirst = false)
        {
            var result = new StringBuilder();

            if (isFirst)
            {
                result.Append($"M{node.X * scaleX} {node.Y * scaleY} ");
            }

            foreach (var dest in node.Nodes)
            {
                result.Append($"L{dest.X * scaleX} {dest.Y * scaleY} ");
                //result.Append($"M{node.X * SvgCellWidth} {node.Y * SvgCellHeight} ");
                result.Append(GenerateSvgPathData(dest, scaleX, scaleY));
            }

            return result.ToString();
        }

        private IEnumerable<Node> ParseFigures()
        {

            var figureId = 1;
            for (var y = 0; y < _buffer.Height; y++)
                for (var x = 0; x < _buffer.Width; x++)
                {
                    if (_buffer.IsFigure(x, y) != 0) continue;

                    var c = _buffer[x, y];
                    if (_lineChars.Contains(c))
                    {

                        var node = new Node(x, y, figureId, c);
                        TraceFrom(node);

                        if (node.Nodes.Any())
                        {
                            yield return node;
                            figureId++;
                        }
                    }
                }
        }

        private void TraceFrom(Node node)
        {
            foreach (var direction in LinesFrom(node.X, node.Y, node.FigureId, node.C))
            {
                var next = TraceLine(node.X, node.Y, node.FigureId, node.C, direction);
                if (next != null)
                {
                    var n = next.Value;
                    node.Nodes.Add(n);
                    TraceFrom(n);
                }
            }
        }



        private Node? TraceLine(int x, int y, int figureId, char c, TraceDirection direction)
        {
            _buffer.SetFigure(x, y, figureId);

            if (direction.HasFlag(TraceDirection.Horizontal)) return TraceHorizontal(x, y, figureId, c, direction.HasFlag(TraceDirection.Right));
            if (direction.HasFlag(TraceDirection.Vertical)) return TraceVertical(x, y, figureId, c, direction.HasFlag(TraceDirection.Down));
            if (direction.HasFlag(TraceDirection.Diagonal)) return TraceDiagonal(x, y, figureId, c, direction.HasFlag(TraceDirection.Right), direction.HasFlag(TraceDirection.Down));
            return null;

        }

        private (bool, Node?) AlreadyTaken(int x, int y, int figureId, char c)
        {
            var existing = _buffer.IsFigure(x, y);
            if (existing == figureId)
                return (true, new Node(x, y, figureId, c));
            return (existing != 0, null);
        }

        private Node? TraceHorizontal(int x, int y, int figureId, char c, bool right)
        {
            // step
            x = right ? x + 1 : x - 1;
            var next = _buffer[x, y];

            switch (AlreadyTaken(x, y, figureId, next))
            {
                case (true, var closingNode):
                    return closingNode;
            }


            // Node, intersection,  soft bend
            if (next == '*' || next == '+' || next == '|' || next == '.' || next == '\'' || next == '<' || next == '>')
            {
                _buffer.SetFigure(x, y, figureId);
                return new Node(x, y, figureId, next);
            }

            if (next == '-')
            {
                _buffer.SetFigure(x, y, figureId);

                //if (c != next)
                //    return new Node(x, y, next);

                // Fork
                if (LinesFrom(x, y, figureId, next).Count() > 1)
                    return new Node(x, y, figureId, next);

                // keep going
                return TraceHorizontal(x, y, figureId, next, right) ?? new Node(x, y, figureId, next);
            }

            return null;
        }
        private Node? TraceVertical(int x, int y, int figureId, char c, bool down)
        {
            // step
            y = down ? y + 1 : y - 1;
            var next = _buffer[x, y];

            switch (AlreadyTaken(x, y, figureId, next))
            {
                case (true, var closingNode):
                    return closingNode;
            }

            // Node, intersection, soft bend
            if (next == '*' || next == '+' || next == '-' || next == '/' || next == '\\' || next == '\'' ||
                (down && next == '\'') || (!down && next == '.') ||
                next == '^' || next == 'v')
            {
                _buffer.SetFigure(x, y, figureId);
                return new Node(x, y, figureId, next);
            }

            if (next == '|')
            {
                _buffer.SetFigure(x, y, figureId);

                //if (c != next)
                //    return new Node(x, y, next);

                // Fork
                if (LinesFrom(x, y, figureId, next).Count() > 1)
                    return new Node(x, y, figureId, next);

                // keep going
                return TraceVertical(x, y, figureId, next, down) ?? new Node(x, y, figureId, next);
            }

            return null;
        }

        private Node? TraceDiagonal(int x, int y, int figureId, char c, bool right, bool down)
        {
            // step
            x = right ? x + 1 : x - 1;
            y = down ? y + 1 : y - 1;

            var next = _buffer[x, y];

            switch (AlreadyTaken(x, y, figureId, next))
            {
                case (true, var closingNode):
                    return closingNode;
            }


            // Node, bend
            if (next == '*' || next == '+' ||
                (down && next == '\'') || (!down && next == '.') ||
                next == 'v' || next == '^' || next == '>' || next == '<')
            {
                _buffer.SetFigure(x, y, figureId);
                return new Node(x, y, figureId, next);
            }

            if ((down == right && next == '\\') || (down != right && next == '/'))
            {
                _buffer.SetFigure(x, y, figureId);

                //if (c != next)
                //    return new Node(x, y, next);

                // Fork
                if (LinesFrom(x, y, figureId, next).Count() > 1)
                    return new Node(x, y, figureId, next);

                // keep going
                return TraceDiagonal(x, y, figureId, next, right, down) ?? new Node(x, y, figureId, next);
            }

            return null;
        }

        private IEnumerable<TraceDirection> LinesFrom(int x, int y, int figureId, char c)
        {
            if (_buffer.IsFigure(x, y - 1) == 0 && _buffer[x, y - 1] == '|')
                yield return TraceDirection.Vertical | TraceDirection.Up;

            if (_buffer.IsFigure(x + 1, y - 1) == 0 && _buffer[x + 1, y - 1] == '/')
                yield return TraceDirection.Diagonal | TraceDirection.Right | TraceDirection.Up;

            if (_buffer.IsFigure(x + 1, y) == 0 && _buffer[x + 1, y] == '-')
                yield return TraceDirection.Horizontal | TraceDirection.Right;

            if (_buffer.IsFigure(x + 1, y + 1) == 0 && _buffer[x + 1, y + 1] == '\\')
                yield return TraceDirection.Diagonal | TraceDirection.Right | TraceDirection.Down;

            if (_buffer.IsFigure(x, y + 1) == 0 && _buffer[x, y + 1] == '|')
                yield return TraceDirection.Vertical | TraceDirection.Down;

            if (_buffer.IsFigure(x - 1, y + 1) == 0 && _buffer[x - 1, y + 1] == '/')
                yield return TraceDirection.Diagonal | TraceDirection.Left | TraceDirection.Down;

            if (_buffer.IsFigure(x - 1, y) == 0 && _buffer[x - 1, y] == '-')
                yield return TraceDirection.Horizontal | TraceDirection.Left;

            if (_buffer.IsFigure(x - 1, y - 1) == 0 && _buffer[x - 1, y - 1] == '\\')
                yield return TraceDirection.Diagonal | TraceDirection.Left | TraceDirection.Up;

            /*
            -1,-1  0,-1  1,-1
            -1, 0  0, 0  1, 0
            -1, 1  0, 1  1, 1
            */
        }
    }
}

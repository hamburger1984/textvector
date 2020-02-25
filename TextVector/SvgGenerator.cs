using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TextVector
{
    public class SvgGenerator
    {
        //public SvgGenerator(int width, int height, int )

        public string ToSvg(IEnumerable<Node> nodes, int width, int height)
        {

            var doc = new XDocument();
            XNamespace ns = ("http://www.w3.org/2000/svg");
            var svg = new XElement(ns + "svg",
                new XAttribute("width", $"{SvgCellWidth * width}"),
                new XAttribute("height", $"{SvgCellHeight * height}"));
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

            foreach (var figure in nodes)
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

        private void NodeToSvg(XContainer el, XNamespace ns, Node node)
        {
            var startCap = SvgLineCap(node.C, true, node.Direction);
            foreach (var dest in node.Nodes)
            {
                var endCap = SvgLineCap(dest.C, false, dest.Direction);
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

        private const int SvgCellWidth = 4;
        private const int SvgCellHeight = 6;
        private const int SvgHalfCellWidth = 2;
        private const int SvgHalfCellHeight = 3;

        //private string GenerateSvgPathData(in Node node, int scaleX, int scaleY, bool isFirst = false)
        //{
        //    var result = new StringBuilder();

        //    if (isFirst)
        //    {
        //        result.Append($"M{node.X * scaleX} {node.Y * scaleY} ");
        //    }

        //    foreach (var dest in node.Nodes)
        //    {
        //        result.Append($"L{dest.X * scaleX} {dest.Y * scaleY} ");
        //        //result.Append($"M{node.X * SvgCellWidth} {node.Y * SvgCellHeight} ");
        //        result.Append(GenerateSvgPathData(dest, scaleX, scaleY));
        //    }

        //    return result.ToString();
        //}


        private string? SvgLineCap(char c, bool start, TraceDirection direction)
        {
            var prefix = start ? "start-" : "end-";

            if (c == 'o')
                return prefix + "round-filled";
            if (c == 'O')
                return prefix + "round-big";
            if (c == '*')
                return prefix + "round-big-filled";
            if (c == '>')
            {
                if (start && direction.HasFlag(TraceDirection.Right))
                    return prefix + "arrow-reverse";
                if (start && !direction.HasFlag(TraceDirection.Right))
                    return prefix + "arrow";
                if (!start && direction.HasFlag(TraceDirection.Right))
                    return prefix + "arrow";
                if (!start && !direction.HasFlag(TraceDirection.Right))
                    return prefix + "arrow-reverse";
            }
            if (c == '<')
            {
                if (start && direction.HasFlag(TraceDirection.Right))
                    return prefix + "arrow";
                if (start && !direction.HasFlag(TraceDirection.Right))
                    return prefix + "arrow-reverse";
                if (!start && direction.HasFlag(TraceDirection.Right))
                    return prefix + "arrow-reverse";
                if (!start && !direction.HasFlag(TraceDirection.Right))
                    return prefix + "arrow";
            }

            if (c == 'v')
                return null;

            if ( c == '^')
                if (!start)
                    return prefix + "arrow";

            return null;
        }
    }
}

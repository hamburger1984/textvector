using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Extensions.FileProviders;
using TextVector.Parsing;

namespace TextVector.Writing
{
    public class SvgGenerator : IWriter
    {
        private const int SvgCellWidth = 12;
        private const int SvgCellHeight = 16;
        private readonly int _height;
        private readonly int _width;

        //private const int SvgHalfCellWidth = 3;
        //private const int SvgHalfCellHeight = 4;

        static SvgGenerator()
        {
            (StyleCssContent, DefXmlContent) = LoadResources("Writing.svg_style.css", "Writing.svg_def.xml");
        }

        private static string DefXmlContent { get; }
        private static string StyleCssContent { get; }

        private static (string content1, string content2) LoadResources(string fileName1, string fileName2)
        {
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            using var reader1 = new StreamReader(embeddedProvider.GetFileInfo(fileName1).CreateReadStream());
            using var reader2 = new StreamReader(embeddedProvider.GetFileInfo(fileName2).CreateReadStream());
            return (reader1.ReadToEnd(), reader2.ReadToEnd());
        }

        public SvgGenerator(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void WriteFile(IEnumerable<Node> figures, string filename)
        {
            var svgContent = ToSvg(figures);
            if (!string.IsNullOrEmpty(filename))
                File.WriteAllText(filename, svgContent);
        }

        public string WriteString(IEnumerable<Node> figures)
        {
            return ToSvg(figures);
        }

        private string ToSvg(IEnumerable<Node> nodes)
        {
            var doc = new XDocument();
            XNamespace ns = "http://www.w3.org/2000/svg";
            var svg = new XElement(ns + "svg",
                new XAttribute("width", $"{SvgCellWidth * (_width + 2)}"),
                new XAttribute("height", $"{SvgCellHeight * (_height + 2)}"));
            doc.Add(svg);

            var def = XElement.Parse(DefXmlContent);
            svg.Add(def);

            var style = new XElement(ns + "style",
                new XAttribute("type", "text/css"),
                new XCData(StyleCssContent));
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

        //private void BuildSvgDefs(XElement defs, XNamespace ns)
        //{
        //    var markers = @"";
        //    defs.Add(XElement.Parse(markers));
        //    //defs.Add(new XElement(ns + "marker",
        //    //        new XAttribute("id", "arrow"),
        //    //        new XAttribute("markerHeight", "7"),
        //    //        new XAttribute("markerWidth", "7"),
        //    //        new XAttribute("orient", "auto-start-reverse"),
        //    //        new XAttribute("refX", "4"),
        //    //        new XAttribute("refY", "2"),
        //    //        new XAttribute("viewBox", "-2 -2 8 8"),
        //    //        new XElement(ns + "polygon",
        //    //            new XAttribute("points", "0,0 0,4 4,2 0,0"))
        //    //    ),
        //    //    new XElement(ns + "marker",
        //    //        new XAttribute("id", "arrow-reverse"),
        //    //        new XAttribute("markerHeight", "7"),
        //    //        new XAttribute("markerWidth", "7"),
        //    //        new XAttribute("orient", "auto-start-reverse"),
        //    //        new XAttribute("refX", "4"),
        //    //        new XAttribute("refY", "2"),
        //    //        new XAttribute("viewBox", "-2 -2 8 8"),
        //    //        new XElement(ns + "polygon",
        //    //            new XAttribute("points", "4,0 4,4 0,2 4,0"))
        //    //    ),
        //    //    new XElement(ns + "marker",
        //    //        new XAttribute("id", "circle-filled"),
        //    //        new XAttribute("markerHeight", "7"),
        //    //        new XAttribute("markerWidth", "7"),
        //    //        new XAttribute("orient", "auto-start-reverse"),
        //    //        new XAttribute("refX", "4"),
        //    //        new XAttribute("refY", "4"),
        //    //        new XAttribute("viewBox", "0 0 8 8"),
        //    //        new XElement(ns + "circle",
        //    //            new XAttribute("class", "filled"),
        //    //            new XAttribute("cx", "4"),
        //    //            new XAttribute("cy", "4"),
        //    //            new XAttribute("r", "2"))
        //    //    )
        //    //);
        //}

        private void NodeToSvg(XContainer el, XNamespace ns, Node node)
        {
            var startCap = SvgLineCap(node.C, true, node.Direction);
            foreach (var dest in node.Nodes)
            {
                var endCap = SvgLineCap(dest.C, false, dest.Direction);
                var classes = string.Join(' ', startCap, endCap);

                el.Add(new XElement(ns + "line",
                    new XAttribute("class", classes),
                    new XAttribute("x1", (node.X + 1) * SvgCellWidth),
                    new XAttribute("y1", (node.Y + 1) * SvgCellHeight),
                    new XAttribute("x2", (dest.X + 1) * SvgCellWidth),
                    new XAttribute("y2", (dest.Y + 1) * SvgCellHeight)));

                NodeToSvg(el, ns, dest);
            }
        }

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
                return prefix + "dot";
            if (c == 'O')
                return prefix + "O";
            if (c == '*')
                return prefix + "big-dot";
            if (c == '>')
                if (start == direction.HasFlag(TraceDirection.Right))
                    return prefix + "arrow-reverse";
                else
                    return prefix + "arrow";
            if (c == '<')
                if (start == direction.HasFlag(TraceDirection.Right))
                    return prefix + "arrow";
                else
                    return prefix + "arrow-reverse";

            if (c == 'v' || c == 'V')
                if (start)
                    return prefix + "arrow-reverse";
                else
                    return prefix + "arrow";

            if (c == '^')
                if (!start)
                    return prefix + "arrow-reverse";
                else
                    return prefix + "arrow";

            return null;
        }
    }
}
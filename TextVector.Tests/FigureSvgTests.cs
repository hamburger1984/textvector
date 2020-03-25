using System.IO;
using System.Linq;
using FluentAssertions;
using TextVector.Buffer;
using TextVector.Parsing;
using TextVector.Writing;
using Xunit;

namespace TextVector.Tests
{
    public class FigureSvgTests
    {
        private void SvgRenderAndCompare(string[] lines, string expectedFile)
        {
            var outputFile = "test.svg";
            if (File.Exists(outputFile)) File.Delete(outputFile);

            var buffer = new TextBuffer(lines);
            var figures = new FigureParser(buffer).Parse();

            var svgGen = new SvgGenerator(buffer.Width, buffer.Height);
            svgGen.WriteFile(outputFile, figures, Enumerable.Empty<Text>());

            File.Exists(outputFile).Should().BeTrue($"Output should exist at {outputFile}.");
            File.Exists(expectedFile).Should().BeTrue($"Reference svg should exist at {expectedFile}.");
            File.ReadAllText(outputFile).Should().BeEquivalentTo(File.ReadAllText(expectedFile));
        }

        [Fact]
        public void Svg_ArrowsHorizontal()
        {
            var lines = new[]
            {
                "   |  >-->  <--<",
                "<--|",
                "   |-->   <--.",
                ">--|         '-->",
                "   '--<",
                "",
                " >--|--<",
                "    |",
                "  <-|->",
                "",
                "   |",
                "<--|",
                "   '-->",
                "",
                "|     |",
                "o--o  O--O",
                "o--   O----*",

            };
            SvgRenderAndCompare(lines, "arrows_horizontal.svg");
        }

        [Fact]
        public void Svg_ArrowsVertical()
        {
            var lines = new[]
            {
                "^  v  |  |  ^  v",
                "|  |  ^  v  |  |",
                "            ^  v"
            };
            SvgRenderAndCompare(lines, "arrows_vertical.svg");
        }

        [Fact]
        public void Svg_TestBox()
        {
            var p = new YetAnotherParser(File.ReadAllLines("./Samples/testbox.txt"));
            p.ParseToSvg("./testbox.svg");
        }
    }

}
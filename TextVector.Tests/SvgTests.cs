using System.IO;
using Xunit;

namespace TextVector.Tests
{
    public class SvgTests
    {
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
            var p = new YetAnotherParser(lines);
            p.ParseToSvg("./arrows_horizontal.svg");
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
            var p = new YetAnotherParser(lines);
            p.ParseToSvg("./arrows_vertical.svg");
        }

        [Fact]
        public void Svg_TestBox()
        {
            var p = new YetAnotherParser(File.ReadAllLines("./Samples/testbox.txt"));
            p.ParseToSvg("./testbox.svg");
        }
    }

}

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
            File.WriteAllText("./arrows_horizontal.svg", p.ParseToSvg());
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
            File.WriteAllText("./arrows_vertical.svg", p.ParseToSvg());
        }

        [Fact]
        public void Svg_TestBox()
        {
            var p = new YetAnotherParser(File.ReadAllLines("./Samples/testbox.txt"));
            var content = p.ParseToSvg();

            Assert.NotNull(content);

            File.WriteAllText("./testbox.svg", content);
        }

    }

}
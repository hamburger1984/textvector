using Xunit;

namespace TextVector.Tests
{
    public class YetAnotherParserTests
    {
        private void TestLines(string expected, params string[] lines)
        {
            var p = new YetAnotherParser(lines);

            var parsed = p.Parse();
            Assert.Equal(expected, parsed);
        }

        [Fact]
        public void Test()
        {
            TestLines("(0,0,-) to (3,0,*)\r\n", "---*");
        }

        [Fact]
        public void HorizontalLines()
        {
            var lines = new[] { "*-- -- - ---*", "", " - --" };
            var p = new YetAnotherParser(lines);

            var parsed = p.Parse();

            Assert.Equal(
                @"1 (0,0,*)
1.1 (2,0,-)
2 (4,0,-)
2.1 (5,0,-)
3 (9,0,-)
3.1 (12,0,*)
4 (3,2,-)
4.1 (4,2,-)
", parsed);
        }

        [Fact]
        public void VerticalLines()
        {
            var expected = @"1 (0,0,|)
1.1 (0,3,|)
2 (2,0,|)
2.1 (2,1,|)
3 (3,2,|)
3.1 (3,3,|)
";
            var lines = new[]
            {
                "| | |",
                "| |",
                "|  |",
                "|  | "
            };
            TestLines(expected, lines);
        }

        [Fact]
        public void Boxes()
        {
            var expected = @"1 (0,0,+)
1.1 (3,0,+)
1.2 (0,3,+)
2 (3,1,|)
2.1 (3,3,+)
3 (1,3,-)
3.1 (2,3,-)
";
            var lines = new[]
            {

                "+--+",
                "|  |",
                "|  |",
                "+--+"
            };
            TestLines(expected, lines);
        }

        [Fact]
        public void Forks()
        {
            var lines = new[]
            {
                "  |",
                " /|\\",
                "/ | \\"
                //"",
                //"  .--",
                //" /---",
                //"-----",
                //"  \\",
                //"   \\",
                //"    \\"
            };
            var p = new YetAnotherParser(lines);
            var parsed = p.Parse();

            Assert.Equal(
                @"line (2,0,|) to (2,1,|) to (2,1,|) to (2,2,|)
", parsed);

        }


        [Fact]
        public void Spiral()
        {
            var lines = new[]
            {
                "----.",
                " .- | ",
                " |  | ",
                " '--' "
            };
            var p = new YetAnotherParser(lines);
            var parsed = p.Parse();

            Assert.Equal(
                @"line (0,0,-) to (4,0,.) to (4,0,.) to (4,3,') to (4,3,') to (1,3,') to (1,3,') to (1,1,.) to (1,1,.) to (2,1,-)
", parsed);
        }
    }
}

using Xunit;

namespace TextVector.Tests
{
    public class YetAnotherParserTests
    {
        private void TestLines(string expected, params string[] lines)
        {
            var p = new YetAnotherParser(lines);

            var parsed = p.ParseToText();
            Assert.Equal(expected, parsed);
        }

        [Fact]
        public void Test()
        {
            TestLines(@"1 (0,0,-)
1 (3,0,*)
", "---*");
        }

        [Fact]
        public void HorizontalLines()
        {
            var expected =
                @"1 (0,0,*)
1 (2,0,-)
2 (4,0,-)
2 (5,0,-)
3 (9,0,-)
3 (12,0,*)
4 (3,2,-)
4 (4,2,-)
5 (0,3,-)
5 (2,3,*)
5 (5,3,*)
5 (8,3,*)
";
            var lines = new[] { "*-- -- - ---*", "", " - --", "--*--*--*" };

            TestLines(expected, lines);
        }

        [Fact]
        public void VerticalLines()
        {
            var expected = @"1 (0,0,|)
1 (0,3,|)
2 (2,0,|)
2 (2,1,|)
3 (3,2,|)
3 (3,3,|)
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
        public void GridRounded()
        {
            var expected =
                @"1 (0,0,.)
1 (2,0,-)
1 (4,0,.)
1 (4,2,|)
1 (4,4,|)
1 (4,6,')
1 (2,6,-)
1 (2,4,|)
1 (2,2,|)
1.1 (2,0,-)
1.2 (4,2,|)
1.3 (0,2,|)
1.3.1 (0,0,.)
1.3.2 (0,4,|)
1.3.2.1 (2,4,|)
1.3.2.1 (4,4,|)
1.3.2.2 (0,6,')
1.3.2.2 (2,6,-)
";
            var lines = new[]
            {

                ".---.",
                "| | |",
                "|-|-|",
                "| | |",
                "|-|-|",
                "| | |",
                "'---'"
            };
            TestLines(expected, lines);

        }

        [Fact]
        public void GridWithCorners()
        {
            var expected = @"1 (0,0,+)
1 (3,0,+)
1 (5,0,+)
1 (5,3,+)
1 (5,5,+)
1 (3,5,+)
1 (3,3,+)
1.1 (3,0,+)
1.2 (5,3,+)
1.3 (0,3,+)
1.3.1 (0,0,+)
1.3.2 (0,5,+)
1.3.2 (3,5,+)
";
            var lines = new[]
            {
                "+--+-+",
                "|  | |",
                "|  | |",
                "+--+-+",
                "|  | |",
                "+--+-+"
            };
            TestLines(expected, lines);
        }


        [Fact]
        public void BoxRounded()
        {
            var expected = @"1 (0,0,.)
1 (4,0,.)
1 (4,3,')
1 (0,3,')
1 (0,0,.)
";
            var lines = new[]
            {
                ".---.",
                "|   |",
                "|   |",
                "'---'"
            };
            TestLines(expected, lines);
        }


        [Fact]
        public void BoxWithCorners()
        {
            var expected = @"1 (0,0,+)
1 (3,0,+)
1 (3,3,+)
1 (0,3,+)
1 (0,0,+)
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
        public void Triangle()
        {
            var expected =
                @"1 (4,0,.)
1 (8,4,')
1 (0,4,')
1 (4,0,.)
";
            var lines = new[]
            {
                "    .",
                "   / \\",
                "  /   \\",
                " /     \\",
                "'-------'"
            };
            TestLines(expected, lines);
        }

        [Fact]
        public void Forks()
        {
            var expected = @"1 (2,0,|)
1.1 (4,2,\)
1.2 (2,2,|)
1.3 (0,2,/)
";
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
            TestLines(expected, lines);
        }


        [Fact]
        public void Spiral()
        {
            var expected = @"1 (0,0,-)
1 (4,0,.)
1 (4,3,')
1 (1,3,')
1 (1,1,.)
1 (2,1,-)
";
            var lines = new[]
            {
                "----.",
                " .- | ",
                " |  | ",
                " '--' "
            };
            TestLines(expected, lines);
        }
    }
}

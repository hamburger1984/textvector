using TextVector.Buffer;
using TextVector.Parsing;
using TextVector.Writing;
using Xunit;

namespace TextVector.Tests
{
    public class FigureTests
    {
        private void TestLines(string expected, params string[] lines)
        {
            var buffer = new TextBuffer(lines);
            var figures = new FigureParser(buffer).Parse();
            var dump = new TextDumper().WriteString(figures);

            Assert.Equal(expected, dump);
        }

        [Fact]
        public void ArrowsDiagonal()
        {
            var expected = @"1 (1,0,/)
1 (0,1,v)
2 (4,0,/)
2 (3,1,^)
3 (6,0,\)
3 (7,1,v)
4 (8,0,\)
4 (9,1,^)
5 (0,3,^)
5 (1,4,\)
6 (3,3,v)
6 (4,4,\)
7 (7,3,v)
7 (6,4,/)
8 (10,3,^)
8 (9,4,/)
";
            var lines = new[]
            {
                @" /  / \ \",
                @"v  ^   v ^",
                "",
                @"^  v   v  ^",
                @" \  \ /  /"
            };
            TestLines(expected, lines);
        }

        [Fact]
        public void ArrowsHorizontal()
        {
            var expected = @"1 (0,0,<)
1 (1,0,-)
2 (3,0,-)
2 (4,0,>)
3 (0,1,>)
3 (1,1,-)
4 (3,1,-)
4 (4,1,<)
5 (0,2,<)
5 (2,2,>)
6 (4,2,>)
6 (6,2,<)
";
            var lines = new[]
            {
                "<- ->",
                ">- -<",
                "<-> >-<"
            };
            TestLines(expected, lines);
        }

        [Fact]
        public void ArrowsVertical()
        {
            var expected = @"1 (0,0,^)
1 (0,2,|)
2 (2,0,v)
2 (2,2,|)
3 (4,0,|)
3 (4,2,v)
4 (6,0,|)
4 (6,2,^)
";
            var lines = new[]
            {
                "^ v | |",
                "| | | |",
                "| | v ^"
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
        public void TrickyArrowsHorizontal()
        {
            var expected = @"1 (0,0,<)
1 (2,0,.)
1 (2,1,')
1 (4,1,>)
";
            var lines = new[]
            {
                "<-.",
                "  '->",
            };

            TestLines(expected, lines);
        }

        [Fact]
        public void TrickyArrowsVertical()
        {
            var expected = @"1 (3,0,|)
1 (3,1,|)
1.1 (3,2,')
1.1 (6,2,>)
1.2 (0,1,<)
";
            var lines = new[]
            {
                "   |",
                "<--|",
                "   '-->"
            };

            TestLines(expected, lines);
        }

        [Fact]
        public void List()
        {
            var expected = @"1 (1,1,+)
1.1 (2,1,-)
1.2 (1,2,+)
1.2.1 (2,2,-)
1.2.2 (1,3,+)
1.2.2.1 (2,3,-)
1.2.2.2 (1,8,+)
1.2.2.2 (2,8,-)
2 (4,4,+)
2.1 (5,4,-)
2.2 (4,5,+)
2.2.1 (5,5,-)
2.2.2 (4,6,')
2.2.2 (5,6,-)
";
            var lines = new[]
            {
                " A",
                " +- 1",
                " +- 2",
                " +- 3",
                " |  +- 3.1",
                " |  +- 3.2",
                " |  '- 3.3",
                " |",
                " +- 4"
            };
            TestLines(expected, lines);
        }

        [Fact]
        public void OLists()
        {
            var expected = @"1 (0,0,|)
1 (0,1,o)
1.1 (2,1,-)
1.2 (0,2,o)
1.2.1 (2,2,-)
1.2.2 (0,3,o)
1.2.2 (2,3,-)
";
            var lines = new[]
            {
                "|",
                "o--A",
                "o--B",
                "o--C",
            };
            TestLines(expected, lines);

        }
    }
}
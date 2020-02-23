using Xunit;

namespace TextVector.Tests
{
    public class SmallParserTests
    {
        [Fact]
        public void HorizontalLines()
        {
            var lines = new[] { "*-- -- - ---*", "", " - --" };
            var p = new SmallParser(lines);

            var parsed = p.Parse();

            Assert.Equal(
                @"line (0,0,*) to (2,0,-)
line (4,0,-) to (5,0,-)
line (9,0,-) to (12,0,*)
line (3,2,-) to (4,2,-)
", parsed);
        }

        [Fact]
        public void VerticalLines()
        {
            var lines = new[]
            {
                "| | |",
                "| |",
                "|  |",
                "|  | "
            };
            var p = new SmallParser(lines);
            var parsed = p.Parse();

            Assert.Equal(
                @"line (0,0,|) to (0,3,|)
line (2,0,|) to (2,1,|)
line (3,2,|) to (3,3,|)
", parsed);
        }

        [Fact]
        public void Boxes()
        {
            var lines = new[]
            {

                "+--+",
                "|  |",
                "|  |",
                "+--+"
            };
            var p = new SmallParser(lines);
            var parsed = p.Parse();

            Assert.Equal(
                @"line (0,0,+) to (3,0,+) to (3,0,+) to (3,3,+) to (3,3,+) to (0,3,+) to (0,3,+) to (0,1,|)
", parsed);
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
            var p = new SmallParser(lines);
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
            var p = new SmallParser(lines);
            var parsed = p.Parse();

            Assert.Equal(
                @"line (0,0,-) to (4,0,.) to (4,0,.) to (4,3,') to (4,3,') to (1,3,') to (1,3,') to (1,1,.) to (1,1,.) to (2,1,-)
", parsed);
        }
    }
}

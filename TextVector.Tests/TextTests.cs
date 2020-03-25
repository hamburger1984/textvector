using TextVector.Buffer;
using TextVector.Parsing;
using TextVector.Writing;
using Xunit;

namespace TextVector.Tests
{
    public class TextTests
    {
        private void TestLines(string expected, params string[] lines)
        {
            var buffer = new TextBuffer(lines);
            var figures = new FigureParser(buffer).Parse();
            var texts = new TextParser(buffer).Parse();
            var dump = new TextDumper().WriteString(figures, texts);

            Assert.Equal(expected, dump);
        }

        [Fact]
        public void FirstText()
        {
            var lines = new[]
            {
                "Knock, knock.. *----> Who is there?",
                "--.  asdf  .----",
                "  |  qwert |",
                "  v        v"
            };

            var expected = @"1 (15,0,*)
1 (20,0,>)
2 (0,1,-)
2 (2,1,.)
2 (2,3,v)
3 (11,1,.)
3.1 (12,1,-)
3.1.1 (12,0,.)
3.1.2 (13,1,-)
3.1.2.1 (13,0,.)
3.1.2.2 (15,1,-)
3.2 (11,3,v)
5 (0,0,Knock, knock)
6 (5,1,asdf  )
7 (5,2,qwert )
8 (22,0,Who is there?)
";

            TestLines(expected, lines);


        }

    }
}

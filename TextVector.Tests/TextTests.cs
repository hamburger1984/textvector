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
        public void JustText_SingleLine()
        {
            var lines = new[] { " ab  cd  ef-gh" };

            var expected = @"1 (1,0,ab)
2 (5,0,cd)
3 (9,0,ef-gh)
";

            TestLines(expected, lines);
        }

        [Fact]
        public void JustText_MultiLine()
        {
            var lines = new[]{
                "This is a Test. Non-repeating non-word chars don't split.",
                "second line  (two parts)",
                " and third",
                "line?! "};

            var expected = @"1 (0,0,This is a Test. Non-repeating non-word chars don't split.)
2 (0,1,second line)
3 (13,1,(two parts))
4 (1,2,and third)
5 (0,3,line?!)
";

            TestLines(expected, lines);
        }


        //[Fact]
        public void FirstText()
        {
            var lines = new[]
            {
                "Knock, knock.  *----> Who is there?",
                "--.  asdf  .----",
                "  |  qwert |",
                "  v        v",
                " ab  cd  ef-gh"
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
4 (0,0,Knock, knock)
5 (21,0,Who is there?)
6 (5,1,asdf)
7 (5,2,qwert)
8 (1,4,ab)
9 (4,4,cd)
";

            TestLines(expected, lines);


        }

    }
}

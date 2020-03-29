using System.Linq;
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
            var textParser = new TextParser(buffer);
            textParser.PreMarkText();
            var figures = new FigureParser(buffer).Parse().ToList();
            var texts = textParser.Parse().ToList();
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


        [Fact]
        public void Mixed_FiguresAndText()
        {
            var lines = new[]
            {
                "Knock, knock.. *----> Who is there?",
                "",
                "--.  asdf  .----",
                "  |  qwert |",
                "  v        v",
                " ab  cd  ef-gh"
            };

            var expected = @"1 (15,0,*)
1 (20,0,>)
2 (0,2,-)
2 (2,2,.)
2 (2,4,v)
3 (11,2,.)
3.1 (15,2,-)
3.2 (11,4,v)
4 (0,0,Knock, knock..)
5 (22,0,Who is there?)
6 (5,2,asdf)
7 (5,3,qwert)
8 (1,5,ab)
9 (5,5,cd)
10 (9,5,ef-gh)
";

            TestLines(expected, lines);


        }

    }
}

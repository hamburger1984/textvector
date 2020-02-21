using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace TextVector.Tests
{
    public class LineTests
    {
        public LineTests()
        {

            _parser = new Parser();
        }

        private Parser _parser;

        [Theory]
        [InlineData("---")]
        [InlineData(" ----- ")]
        [InlineData(@"-----------

  --  ---- -------")]
        public void DashesBecomeHorizontalLine(string value)
        {
            var parsed = _parser.Parse(value);

            foreach (var l in parsed)
            {
                Assert.Equal(Element.Line, l.Element);
                Assert.Equal(l.Y, l.Y2);
                Assert.NotEqual(l.X, l.X2);
            }
        }


        [Theory]
        [InlineData("*Test* *more testing* _more testing_")]
        [InlineData("This is a test")]
        [InlineData("Test-Test-Test -test-")]
        [InlineData("-")]
        public void Text(string value)
        {
            var parsed = _parser.Parse(value).ToArray();

            Assert.True(parsed.All(p => p.Element == Element.Text));
        }

        [Fact]
        public void LineLengths()
        {
            var text = "--- - ---- -----";
            var lines = _parser.Parse(text).ToArray();

            Assert.Equal(4, lines.Length);

            Assert.Equal(3, lines[0].X2 - lines[0].X);
            Assert.Equal(0, lines[0].Y2 - lines[0].Y);

            Assert.Equal("-", lines[1].Value);

            Assert.Equal(4, lines[2].X2 - lines[2].X);
            Assert.Equal(0, lines[2].Y2 - lines[2].Y);

            Assert.Equal(5, lines[3].X2 - lines[3].X);
            Assert.Equal(0, lines[3].Y2 - lines[3].Y);

        }

    }
}

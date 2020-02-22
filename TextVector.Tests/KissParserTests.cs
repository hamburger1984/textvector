using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace TextVector.Tests
{
    public class KissParserTests
    {
        private readonly ITestOutputHelper _output;

        public KissParserTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ParseTestBox()
        {
            var lines = File.ReadAllLines("./Samples/testbox.txt");
            var parser = new KissParser();


            var parsed = parser.Parse(lines, m => _output.WriteLine(m));

        }


        [Fact]
        public void LoadIntoBuffer()
        {
            var buffer = new TextBuffer(File.ReadAllLines("./Samples/testbox.txt"));

            Assert.Equal(new[]
            {
                '\0', ' ', ' ',
                '\0', '<', '-',
                '\0', ' ', ' '
            }, buffer.Neighborhood(0, 14));

            Assert.Equal(new[]
            {
                '\0','\0','\0',
                '>', ' ', '\0',
                ' ', ' ', '\0'
            }, buffer.Neighborhood(42, 0));

            Assert.Equal('0', buffer[15, 32]);
            Assert.Equal(new[]
            {
                '.', '─', '.',
                ' ', '0', ' ',
                '`', '-', '\''
            }, buffer.Neighborhood(15, 32));
            Assert.Equal(new[]
            {
                ' ', ' ', ' ', ' ', ' ',
                ' ', '.', '─', '.', ' ',
                '(', ' ', '0', ' ', ')',
                ' ', '`', '-', '\'', ' ',
                ' ', ' ', ' ', ' ', ' '
            }, buffer.Neighborhood(15, 32, 2));
        }
    }
}

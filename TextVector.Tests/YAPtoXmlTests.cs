using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;

namespace TextVector.Tests
{
    public class YAPtoXmlTests
    {
        [Fact]
        public void One()
        {
            var p = new YetAnotherParser(File.ReadAllLines("./Samples/testbox.txt"));
            var content = p.ParseToSvg();

            Assert.NotNull(content);

            File.WriteAllText("./testbox.svg", content);
        }
    }
}

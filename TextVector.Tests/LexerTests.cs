using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace TextVector.Tests
{
    public class LexerTests
    {
        [Fact]
        public void SimpleLexer()
        {

            var content = @" --- # Testing
*More Testing* with _formatting_!

  ---->  B
C123-456 --|-- o----+ *******";

            var lexer = new Lexer(new StringReader(content));

            foreach (var token in lexer.Tokens())
            {
                Assert.True((token.Kind == TokenKind.Eof) == (token.Content == null));
            }
        }
    }
}

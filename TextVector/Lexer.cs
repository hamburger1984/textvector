using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextVector
{
    public class Lexer
    {
        public Lexer(TextReader reader)
        {
            _lines = new List<ReadOnlyMemory<char>>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                _lines.Add((line + "\n").AsMemory());
            }
            _lines.Add("\0".AsMemory());
        }
        private int _x;
        private int _y;
        private readonly List<ReadOnlyMemory<char>> _lines;
        private ReadOnlySpan<char> _line => _lines[_y].Span;
        private char _ch => _line[_x];
        private char _next
        {
            get
            {
                var line = _line;
                if (_x + 1 < line.Length) return line[_x + 1];
                if (_y + 1 < _lines.Count) return _lines[_y + 1].Span[0];
                return '\0';
            }
        }

        private char _prev
        {
            get
            {
                if (_x == 0) return _y == 0 ? '\0' : '\n';
                return _line[_x - 1];
            }
        }

        private bool IsEof() => _ch == '\0';
        private bool IsWhiteSpace() => char.IsWhiteSpace(_ch) && !IsNewLine();
        private bool IsNewLine() => _ch == '\n';

        private bool IsWordPart() => char.IsLetterOrDigit(_ch) ||
                                     ((_ch == '-' || _ch == '_' || _ch == '*' || _ch == ' ') &&
                                      (char.IsLetterOrDigit(_next) || char.IsLetterOrDigit(_prev)));

        private bool IsLinePart() => _ch == '-' || _ch == '*' || _ch == '/' || _ch == '|' || _ch == '\\' ||
                                     _ch == '*' || _ch == '\'' || _ch == '.' || _ch == '+' ||
                                     (_ch == 'o' && _next == '-');

        public IEnumerable<Token> Tokens()
        {
            while (!IsEof())
            {
                yield return LexToken();
            }
            yield return new Token(TokenKind.Eof, _y, _x);
        }

        private Token LexToken()
        {
            if (IsEof())
                return new Token(TokenKind.Eof, _y, _x);
            if (IsNewLine())
                return ScanNewLine();
            if (IsWhiteSpace())
                return ScanWhiteSpace();
            if (IsLinePart())
                return ScanLine();
            if (IsWordPart())
                return ScanText();

            Advance();
            return CreateToken(TokenKind.Other);

        }

        private void Advance()
        {
            _x++;
        }

        private void DoNewLine()
        {
            _y++;
            _x = 0;
            _tokenStart = 0;
        }

        private Token ScanWhiteSpace()
        {
            do
            {
                Advance();
            } while (IsWhiteSpace());

            return CreateToken(TokenKind.WhiteSpace);
        }

        private Token ScanNewLine()
        {
            Advance();
            var token = CreateToken(TokenKind.NewLine);
            DoNewLine();
            return token;
        }

        private Token ScanLine()
        {
            do
            {
                Advance();
            } while (IsLinePart());

            return CreateToken(TokenKind.Line);
        }

        private Token ScanText()
        {
            do
            {
                Advance();
            } while (IsWordPart());

            return CreateToken(TokenKind.Text);
        }

        private int _tokenStart;

        private Token CreateToken(TokenKind kind)
        {
            var content = _line.Slice(_tokenStart, _x - _tokenStart).ToString();
            var token = new Token(kind, _y, _tokenStart, _x, content);
            _tokenStart = _x;
            return token;
        }
    }

    public enum TokenKind
    {
        Eof,
        WhiteSpace,
        NewLine,
        Other,
        Text,
        Line
    }

    public readonly struct Token
    {
        public Token(TokenKind kind, int y, int x, int? x2 = null, string? content = null)
        {
            Kind = kind;
            Y = y;
            X = x;
            X2 = x2;
            Content = content;
        }

        public TokenKind Kind { get; }
        public int Y { get; }
        public int X { get; }
        public int? X2 { get; }
        public string? Content { get; }
    }
}

using GLSLLanguageIntegration.Spans;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Tokens
{
    public class TokenBuilder
    {
        public int Position { get; set; }

        private ITextSnapshot _textSnapshot;

        public TokenBuilder(ITextBuffer textBuffer)
        {
            _textSnapshot = textBuffer.CurrentSnapshot;
        }

        public IEnumerable<SnapshotSpan> GetTokenSpans(int start, int end)
        {
            Position = start;
            var tokenStart = Position;

            while (Position < end)
            {
                char character = _textSnapshot[Position];
                Position++;

                if (char.IsWhiteSpace(character))
                {
                    if (Position > tokenStart + 1)
                    {
                        yield return new SnapshotSpan(_textSnapshot, tokenStart, Position - 1 - tokenStart);  
                    }

                    tokenStart = Position;
                }
                else if (IsTokenTerminator(character))
                {
                    if (Position > tokenStart + 1)
                    {
                        Position--;
                    }
                    else if (Position < _textSnapshot.Length && IsOperatorToken(_textSnapshot.GetText(Position - 1, 2)))
                    {
                        Position++;
                    }

                    yield return new SnapshotSpan(_textSnapshot, tokenStart, Position - tokenStart);
                    tokenStart = Position;
                }
            }

            if (Position > tokenStart)
            {
                if (!_textSnapshot.GetText(tokenStart, Position - tokenStart).All(t => char.IsWhiteSpace(t)))
                {
                    yield return new SnapshotSpan(_textSnapshot, tokenStart, Position - tokenStart);
                }
            }
        }

        private bool IsOperatorToken(string value) => value == "//"
            || value == "/*"
            || value == "=="
            || value == "&&"
            || value == "||";

        private bool IsTokenTerminator(char character) => !char.IsLetterOrDigit(character) && character != '_';
    }
}

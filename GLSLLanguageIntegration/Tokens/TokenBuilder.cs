using Microsoft.VisualStudio.Text;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Tokens
{
    public class TokenBuilder
    {
        public int Position { get; set; }
        public bool IsCompleted { get; private set; } = false;

        private ITextSnapshot _textSnapshot;
        private string _text;

        public TokenBuilder(ITextBuffer textBuffer)
        {
            _textSnapshot = textBuffer.CurrentSnapshot;
            _text = _textSnapshot.GetText();

            textBuffer.Changed += (s, args) =>
            {
                if (args.After == _textSnapshot)
                {
                    // TODO - Avoid re-parsing the entire buffer by only parsing the relevant spans
                    //ParseBuffer();// args.Changes);
                }
            };
        }

        public SnapshotSpan GetNextTokenSpan()
        {
            var tokenStart = Position;

            while (Position < _text.Length)
            {
                char character = _text[Position];
                Position++;

                if (char.IsWhiteSpace(character))
                {
                    if (Position > tokenStart + 1)
                    {
                        return new SnapshotSpan(_textSnapshot, tokenStart, Position - 1 - tokenStart);
                    }
                    else
                    {
                        tokenStart = Position;
                    }
                }
                else if (IsTokenTerminator(character))
                {
                    if (Position > tokenStart + 1)
                    {
                        Position--;
                    }
                    else if (Position < _text.Length && IsOperatorToken(_text.Substring(Position - 1, 2)))
                    {
                        Position++;
                    }

                    return new SnapshotSpan(_textSnapshot, tokenStart, Position - tokenStart);
                }
            }

            IsCompleted = true;
            return new SnapshotSpan(_textSnapshot, tokenStart, Position - tokenStart);
        }

        private bool IsOperatorToken(string value)
        {
            return value == "//"
                || value == "/*"
                || value == "=="
                || value == "&&"
                || value == "||";
        }

        public IEnumerable<SnapshotSpan> GetTokenSpans()
        {
            var tokenLength = 0;
            var text = _textSnapshot.GetText();

            // Process until we reach a token terminator
            for (var i = 0; i < text.Length; i++)
            {
                if (char.IsWhiteSpace(text[i]))
                {
                    // In this case, we do not want to tokenize this character, but we want to reset the tokenStart
                    tokenLength++;
                }
                else if (IsTokenTerminator(text[i]))
                {
                    // In this case, we want to tokenize AND reset the tokenStart
                    yield return new SnapshotSpan(_textSnapshot, i - tokenLength, i);
                    tokenLength++;
                }
            }
        }

        private bool IsTokenTerminator(char character) => !char.IsLetterOrDigit(character) && character != '_';
    }
}

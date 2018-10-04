using GLSLLanguageIntegration.Spans;
using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Tokens
{
    public class TokenBuilder
    {
        public int Position { get; set; }

        private ITextSnapshot _textSnapshot;
        private StatementCollection _statements;
        private INormalizedTextChangeCollection _textChanges;

        private int _textChangeIndex = 0;
        private int _positionDelta = 0;

        // TODO - Handle ITextChange appropriately
        // The TokenBuilder should take in a set of text changes, and check each of these text changes for token spans until none are remaining
        // The first buffer parse should be where the entire snapshot is considered one big text change
        public TokenBuilder(ITextBuffer textBuffer)
        {
            _textSnapshot = textBuffer.CurrentSnapshot;
        }

        public TokenBuilder(ITextBuffer textBuffer, StatementCollection statements, INormalizedTextChangeCollection textChanges)
        {
            _textSnapshot = textBuffer.CurrentSnapshot;
            _statements = statements;
            _textChanges = textChanges;
        }

        public IEnumerable<SnapshotSpan> GetTokenSpans()
        {
            var tokenStart = Position;

            while (Position < _textSnapshot.Length)
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

        public bool TryGetNextTokenSpan(out SnapshotSpan span)
        {
            if (_textChanges != null)
            {
                return TryGetNextTokenSpanFromTextChanges(out span);
            }

            var tokenStart = Position;

            if (tokenStart >= _textSnapshot.Length)
            {
                span = new SnapshotSpan();
                return false;
            }
            else
            {
                while (Position < _textSnapshot.Length)
                {
                    char character = _textSnapshot[Position];
                    Position++;

                    if (char.IsWhiteSpace(character))
                    {
                        if (Position > tokenStart + 1)
                        {
                            span = new SnapshotSpan(_textSnapshot, tokenStart, Position - 1 - tokenStart);
                            return true;
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
                        else if (Position < _textSnapshot.Length && IsOperatorToken(_textSnapshot.GetText(Position - 1, 2)))
                        {
                            Position++;
                        }

                        span = new SnapshotSpan(_textSnapshot, tokenStart, Position - tokenStart);
                        return true;
                    }
                }

                span = new SnapshotSpan(_textSnapshot, tokenStart, Position - tokenStart);
                return true;
            }
        }

        private bool TryGetNextTokenSpanFromTextChanges(out SnapshotSpan span)
        {
            while (_textChangeIndex < _textChanges.Count)
            {
                // TextChange has the following useful properties -> OldPosition, NewPosition, OldLength, NewLength
                var textChange = _textChanges[_textChangeIndex];

                // We can use NewPosition to determine if this textChange is starting in the middle of a statement, or in the middle of a token
                // We at least have to store all statement spans in the document to determine what position we need to start over from (start processing at start of current statement)
                var position = textChange.NewPosition;
                var statement = _statements.GetStatementForPosition(position);

                // For now, we can just determine to RE-PARSE the entirety of the current statement that we are in
                // This is easy enough, but we ALSO need to remove any tag spans that relate to this statement, since we don't want to end up re-adding those tag spans
                Position = statement.Span.Start;

                // Now we generate token spans until 1) we have parsed at least the full textChange.NewLength amount, and 2) we finish whatever statement we are currently on
                var isStatementParsed = false;

                var tokenStart = Position;

                if (tokenStart >= _textSnapshot.Length)
                {
                    span = new SnapshotSpan();
                    return false;
                }
                else
                {
                    while (Position < _textSnapshot.Length)
                    {
                        char character = _textSnapshot[Position];
                        Position++;

                        if (char.IsWhiteSpace(character))
                        {
                            if (Position > tokenStart + 1)
                            {
                                span = new SnapshotSpan(_textSnapshot, tokenStart, Position - 1 - tokenStart);
                                return true;
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
                            else if (Position < _textSnapshot.Length && IsOperatorToken(_textSnapshot.GetText(Position - 1, 2)))
                            {
                                Position++;
                            }

                            isStatementParsed = true;

                            span = new SnapshotSpan(_textSnapshot, tokenStart, Position - tokenStart);
                            return true;
                        }
                    }

                    span = new SnapshotSpan(_textSnapshot, tokenStart, Position - tokenStart);
                    return true;
                }


                // We need to process this specific textChange span for new token spans
                // HOWEVER, we first need to check to see what statement this textChange span falls into
                // We don't currently store statements, so how should we go about doing this?
                // FIRST, we want to check to see if this text change falls inside of a comment or a preprocessor
            }

            span = new SnapshotSpan();
            return false;
        }

        private bool IsOperatorToken(string value)
        {
            return value == "//"
                || value == "/*"
                || value == "=="
                || value == "&&"
                || value == "||";
        }

        private bool IsTokenTerminator(char character) => !char.IsLetterOrDigit(character) && character != '_';

        /*public IEnumerable<SnapshotSpan> GetTokenSpans()
        {
            var tokenStart = Position;

            while (Position < _textSnapshot.Length)
            {
                char character = _textSnapshot[Position];

                if (char.IsWhiteSpace(character))
                {
                    if (Position > tokenStart + 1)
                    {
                        yield return new SnapshotSpan(_textSnapshot, tokenStart, Position - 1 - tokenStart);
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
                    else if (Position < _textSnapshot.Length && IsOperatorToken(_textSnapshot.GetText(Position - 1, 2)))
                    {
                        Position++;
                    }

                    yield return new SnapshotSpan(_textSnapshot, tokenStart, Position - tokenStart);
                }

                Position++;
            }

            if (Position > tokenStart)
            {
                yield return new SnapshotSpan(_textSnapshot, tokenStart, Position - tokenStart);
            }
        }*/

        /*public IEnumerable<SnapshotSpan> GetTokenSpans()
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
        }*/
    }
}

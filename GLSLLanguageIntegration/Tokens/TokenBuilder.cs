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
        private StatementCollection _statements;
        private INormalizedTextChangeCollection _textChanges;

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

        public IEnumerable<SnapshotSpan> GetTokenSpansForTextChanges()
        {
            foreach (var textChange in _textChanges)
            {
                // Get the statement/preprocessor that matches this position, 
                var span = _statements.Reprocess(textChange.NewPosition, textChange.NewEnd);

                // For now, we can just determine to RE-PARSE the entirety of the current statement that we are in
                // This is easy enough, but we ALSO need to remove any tag spans that relate to this statement, since we don't want to end up re-adding those tag spans
                if (Position <= span.Start)
                {
                    Position = span.Start;

                    // Now we generate token spans until
                    //  1) we have parsed at least the full textChange.NewLength amount, and
                    //  2) we finish whatever statement we are currently on
                    var tokenStart = Position;

                    while (Position - span.Start < textChange.NewLength || Position < span.End)//Position - tokenStart < textChange.NewLength && Position < statementEnd)
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

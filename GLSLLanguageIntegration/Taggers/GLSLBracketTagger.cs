using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tags;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLBracketTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Bracket;

        private List<SnapshotSpan> _bracketSpans = new List<SnapshotSpan>();
        private SpanBuilder _bracketBuilder = new SpanBuilder();

        public GLSLSpanResult Match(string token, int position, SnapshotSpan span)
        {
            _bracketBuilder.Snapshot = span.Snapshot;

            var result = new GLSLSpanResult(TOKEN_TYPE, span);
            result.AddSpans<GLSLOutlineTag>(_bracketSpans);

            if (token.Contains("{"))
            {
                int tokenIndex = token.IndexOf("{");
                int start = position - token.Length + tokenIndex;

                if (!_bracketBuilder.Start.HasValue)
                {
                    _bracketBuilder.Start = start;
                    string text = span.Snapshot.GetText();

                    int end = GetClosingBracketPosition(text, start); //_bracketBuilder.ConsumeUntil(text, start, "}");
                    if (end >= 0)
                    {
                        _bracketBuilder.End = end + 1;
                        var bracketSpan = _bracketBuilder.ToSpan();
                        _bracketBuilder.Clear();

                        int outlineEnd = _bracketBuilder.ConsumeUntil(text, start, "\r", "\n");
                        if (outlineEnd >= 0)
                        {
                            var bracketTagSpan = new TagSpan<IGLSLTag>(bracketSpan, new GLSLOutlineTag(TOKEN_TYPE, text.Substring(start, outlineEnd - start) + "...}"));
                            _bracketSpans.Add(bracketTagSpan.Span);
                            result.AddSpan(bracketTagSpan);
                        }
                    }
                }
            }

            return result;
        }

        private int GetClosingBracketPosition(string text, int start)
        {
            int position = _bracketBuilder.ConsumeUntil(text, start, "{", "}");
            if (position >= 0)
            {
                switch (text[position])
                {
                    case '{':
                        int end = GetClosingBracketPosition(text, position);
                        int doubleEnd = GetClosingBracketPosition(text, end);
                        return doubleEnd;
                    case '}':
                        return position;
                }
            }

            throw new ArgumentException("Closing bracket not found in text");
        }
    }
}

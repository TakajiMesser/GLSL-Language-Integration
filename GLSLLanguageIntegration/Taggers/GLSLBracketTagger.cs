using GLSLLanguageIntegration.Outlining;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLBracketTagger : IGLSLTagger
    {
        public const GLSLTokenTypes PARENTHESIS_TOKEN_TYPE = GLSLTokenTypes.Parenthesis;
        public const GLSLTokenTypes CURLY_TOKEN_TYPE = GLSLTokenTypes.CurlyBracket;
        public const GLSLTokenTypes SQUARE_TOKEN_TYPE = GLSLTokenTypes.SquareBracket;

        private List<TagSpan<IGLSLTag>> _parenthesisSpans = new List<TagSpan<IGLSLTag>>();
        private List<TagSpan<IGLSLTag>> _curlyBracketSpans = new List<TagSpan<IGLSLTag>>();
        private List<TagSpan<IGLSLTag>> _squareBracketSpans = new List<TagSpan<IGLSLTag>>();

        private SpanBuilder _parenthesisBuilder = new SpanBuilder();
        private SpanBuilder _curlyBracketBuilder = new SpanBuilder();
        private SpanBuilder _squareBracketBuilder = new SpanBuilder();

        public GLSLSpanResult Match(string token, int position, SnapshotSpan span)
        {
            _parenthesisBuilder.Snapshot = span.Snapshot;
            _curlyBracketBuilder.Snapshot = span.Snapshot;
            _squareBracketBuilder.Snapshot = span.Snapshot;

            var result = new GLSLSpanResult(CURLY_TOKEN_TYPE, span);
            /*string text = span.Snapshot.GetText();

            for (var i = 0; i < token.Length; i++)
            {
                switch (token[i])
                {
                    case '(':
                        var parenthesisSpan = GetSpan(_parenthesisBuilder, text, position - token.Length + i, '(', ')');
                        if (parenthesisSpan.HasValue)
                        {
                            var collapseText = GetCollapseText(_parenthesisBuilder, text, position - token.Length + i, ')');
                            _parenthesisSpans.Add(new TagSpan<IGLSLTag>(span, new GLSLOutlineTag(PARENTHESIS_TOKEN_TYPE, collapseText)));
                        }
                        break;
                    case '{':
                        var curlySpan = GetSpan(_curlyBracketBuilder, text, position - token.Length + i, '(', ')');
                        if (curlySpan.HasValue)
                        {
                            var collapseText = GetCollapseText(_curlyBracketBuilder, text, position - token.Length + i, ')');
                            _curlyBracketSpans.Add(new TagSpan<IGLSLTag>(span, new GLSLOutlineTag(CURLY_TOKEN_TYPE, collapseText)));
                        }
                        break;
                    case '[':
                        var squareSpan = GetSpan(_squareBracketBuilder, text, position - token.Length + i, '(', ')');
                        if (squareSpan.HasValue)
                        {
                            var collapseText = GetCollapseText(_squareBracketBuilder, text, position - token.Length + i, ')');
                            _squareBracketSpans.Add(new TagSpan<IGLSLTag>(span, new GLSLOutlineTag(SQUARE_TOKEN_TYPE, collapseText)));
                        }
                        break;
                }
            }*/

            MatchParentheses(token, position, span);
            MatchCurlyBrackets(token, position, span);
            MatchSquareBrackets(token, position, span);

            result.AddSpans(_parenthesisSpans);
            result.AddSpans(_curlyBracketSpans);
            result.AddSpans(_squareBracketSpans);

            return result;
        }

        public int GetScope(SnapshotSpan span)
        {
            int scope = 0;

            foreach (var bracketSpan in _curlyBracketSpans)
            {
                var translatedSpan = bracketSpan.Span.Translated(span.Snapshot);

                if (translatedSpan.OverlapsWith(span))
                {
                    scope++;
                }
            }

            return scope;
        }

        public void Clear()
        {
            _parenthesisSpans.Clear();
            _curlyBracketSpans.Clear();
            _squareBracketSpans.Clear();

            _parenthesisBuilder.Clear();
            _curlyBracketBuilder.Clear();
            _squareBracketBuilder.Clear();
        }

        private SnapshotSpan? GetSpan(SpanBuilder spanBuilder, string text, int position, char openingChar, char closingChar)
        {
            if (!spanBuilder.Start.HasValue)
            {
                spanBuilder.Start = position;

                int end = GetClosingPosition(text, position, openingChar, closingChar, spanBuilder);
                if (end >= 0)
                {
                    spanBuilder.End = end + 1;
                    var span = spanBuilder.ToSpan();
                    spanBuilder.Clear();

                    return span;
                }
            }

            return null;
        }

        private string GetCollapseText(SpanBuilder spanBuilder, string text, int position, char closingChar)
        {
            int spanEnd = spanBuilder.End.Value - 1;

            int outlineEnd = spanBuilder.ConsumeUntil(text, position, "\r", "\n");
            if (outlineEnd >= 0)
            {
                var collapseText = new StringBuilder();
                if (spanEnd > outlineEnd)
                {
                    collapseText.Append(text.Substring(position, outlineEnd - position));
                }
                collapseText.Append("..." + closingChar);

                return collapseText.ToString();
            }

            return GLSLOutlineTag.COLLAPSE_TEXT;
        }

        private void MatchParentheses(string token, int position, SnapshotSpan span)
        {
            if (token.Contains("("))
            {
                int tokenIndex = token.IndexOf("(");
                int start = position - token.Length + tokenIndex;

                if (!_parenthesisBuilder.Start.HasValue)
                {
                    _parenthesisBuilder.Start = start;
                    string text = span.Snapshot.GetText();

                    int end = GetClosingPosition(text, start, '(', ')', _parenthesisBuilder);
                    if (end >= 0)
                    {
                        _parenthesisBuilder.End = end + 1;
                        var parenthesisSpan = _parenthesisBuilder.ToSpan();
                        _parenthesisBuilder.Clear();

                        int outlineEnd = _parenthesisBuilder.ConsumeUntil(text, start, "\r", "\n");
                        if (outlineEnd >= 0 && outlineEnd < end)
                        {
                            var collapseText = text.Substring(start, outlineEnd - start) + "...)";

                            var tagSpan = new TagSpan<IGLSLTag>(parenthesisSpan, new GLSLOutlineTag(PARENTHESIS_TOKEN_TYPE, collapseText));
                            _parenthesisSpans.Add(tagSpan);
                        }
                    }
                }
            }
        }

        private void MatchCurlyBrackets(string token, int position, SnapshotSpan span)
        {
            if (token.Contains("{"))
            {
                int tokenIndex = token.IndexOf("{");
                int start = position - token.Length + tokenIndex;

                if (!_curlyBracketBuilder.Start.HasValue)
                {
                    _curlyBracketBuilder.Start = start;
                    string text = span.Snapshot.GetText();

                    int end = GetClosingPosition(text, start, '{', '}', _curlyBracketBuilder);
                    if (end >= 0)
                    {
                        _curlyBracketBuilder.End = end + 1;
                        var bracketSpan = _curlyBracketBuilder.ToSpan();
                        _curlyBracketBuilder.Clear();

                        int outlineEnd = _curlyBracketBuilder.ConsumeUntil(text, start, "\r", "\n");
                        if (outlineEnd >= 0 && outlineEnd < end)
                        {
                            var collapseText = text.Substring(start, outlineEnd - start) + "...}";

                            var tagSpan = new TagSpan<IGLSLTag>(bracketSpan, new GLSLOutlineTag(CURLY_TOKEN_TYPE, collapseText));
                            _curlyBracketSpans.Add(tagSpan);
                        }
                    }
                }
            }
        }

        private void MatchSquareBrackets(string token, int position, SnapshotSpan span)
        {
            if (token.Contains("["))
            {
                int tokenIndex = token.IndexOf("[");
                int start = position - token.Length + tokenIndex;

                if (!_squareBracketBuilder.Start.HasValue)
                {
                    _squareBracketBuilder.Start = start;
                    string text = span.Snapshot.GetText();

                    int end = GetClosingPosition(text, start, '[', ']', _squareBracketBuilder);
                    if (end >= 0)
                    {
                        _squareBracketBuilder.End = end + 1;
                        var bracketSpan = _squareBracketBuilder.ToSpan();
                        _squareBracketBuilder.Clear();

                        int outlineEnd = _squareBracketBuilder.ConsumeUntil(text, start, "\r", "\n");
                        if (outlineEnd >= 0 && outlineEnd < end)
                        {
                            var collapseText = text.Substring(start, outlineEnd - start) + "...]";

                            var tagSpan = new TagSpan<IGLSLTag>(bracketSpan, new GLSLOutlineTag(SQUARE_TOKEN_TYPE, collapseText));
                            _squareBracketSpans.Add(tagSpan);
                        }
                    }
                }
            }
        }

        private int GetClosingPosition(string text, int start, char openingCharacter, char closingCharacter, SpanBuilder spanBuilder)
        {
            int position = spanBuilder.ConsumeUntil(text, start, openingCharacter.ToString(), closingCharacter.ToString());
            if (position >= 0)
            {
                if (text[position] == openingCharacter)
                {
                    int end = GetClosingPosition(text, position, openingCharacter, closingCharacter, spanBuilder);
                    int doubleEnd = GetClosingPosition(text, end, openingCharacter, closingCharacter, spanBuilder);
                    return doubleEnd;
                }
                else
                {
                    return position;
                }
            }

            throw new ArgumentException("Closing bracket not found in text");
        }
    }
}

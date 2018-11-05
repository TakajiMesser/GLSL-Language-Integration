using GLSLLanguageIntegration.Outlining;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;
using System.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLBracketTagger : IGLSLTagger
    {
        public const GLSLTokenTypes PARENTHESIS_TOKEN_TYPE = GLSLTokenTypes.Parenthesis;
        public const GLSLTokenTypes CURLY_TOKEN_TYPE = GLSLTokenTypes.CurlyBracket;
        public const GLSLTokenTypes SQUARE_TOKEN_TYPE = GLSLTokenTypes.SquareBracket;

        private List<TagSpan<GLSLOutlineTag>> _parenthesisSpans = new List<TagSpan<GLSLOutlineTag>>();
        private List<TagSpan<GLSLOutlineTag>> _curlyBracketSpans = new List<TagSpan<GLSLOutlineTag>>();
        private List<TagSpan<GLSLOutlineTag>> _squareBracketSpans = new List<TagSpan<GLSLOutlineTag>>();

        private SpanBuilder _parenthesisBuilder = new SpanBuilder();
        private SpanBuilder _curlyBracketBuilder = new SpanBuilder();
        private SpanBuilder _squareBracketBuilder = new SpanBuilder();

        private Scope _rootScope;

        public TokenTagCollection Match(SnapshotSpan span)
        {
            _parenthesisBuilder.Snapshot = span.Snapshot;
            _curlyBracketBuilder.Snapshot = span.Snapshot;
            _squareBracketBuilder.Snapshot = span.Snapshot;

            var tokenTags = new TokenTagCollection(span);

            switch (span.GetText())
            {
                case "(":
                    return MatchParentheses(span);
                case "{":
                    return MatchCurlyBrackets(span);
                case "[":
                    return MatchSquareBrackets(span);
                case ")":
                    tokenTags.SetClassifierTag(GLSLTokenTypes.Parenthesis);
                    break;
                case "}":
                    tokenTags.SetClassifierTag(GLSLTokenTypes.CurlyBracket);
                    break;
                case "]":
                    tokenTags.SetClassifierTag(GLSLTokenTypes.SquareBracket);
                    break;
            }

            /*result.AddTagSpans(_parenthesisSpans);
            result.AddTagSpans(_curlyBracketSpans);
            result.AddTagSpans(_squareBracketSpans);*/

            return tokenTags;
        }

        public Scope GetScope(SnapshotSpan span) => GetRootScope(span).GetMatchingScope(span);

        public void Translate(ITextSnapshot textSnapshot) => _rootScope.Translate(textSnapshot);

        public void Clear()
        {
            _parenthesisSpans.Clear();
            _curlyBracketSpans.Clear();
            _squareBracketSpans.Clear();

            _parenthesisBuilder.Clear();
            _curlyBracketBuilder.Clear();
            _squareBracketBuilder.Clear();

            _rootScope = null;
        }

        private Scope GetRootScope(SnapshotSpan span)
        {
            _rootScope = _rootScope ?? new Scope(span.Snapshot.FullSpan());
            return _rootScope;
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

        private TokenTagCollection MatchParentheses(SnapshotSpan span)
        {
            var tokenTags = new TokenTagCollection(span);
            tokenTags.SetClassifierTag(GLSLTokenTypes.Parenthesis);

            int start = span.Start;

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

                        var tagSpan = new TagSpan<GLSLOutlineTag>(parenthesisSpan, new GLSLOutlineTag(PARENTHESIS_TOKEN_TYPE, collapseText));
                        tokenTags.AddOutlineTagSpan(tagSpan);
                        _parenthesisSpans.Add(tagSpan);
                    }
                }
            }

            return tokenTags;
        }

        private TokenTagCollection MatchCurlyBrackets(SnapshotSpan span)
        {
            var tokenTags = new TokenTagCollection(span);
            tokenTags.SetClassifierTag(GLSLTokenTypes.CurlyBracket);

            int start = span.Start;

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

                        var tagSpan = new TagSpan<GLSLOutlineTag>(bracketSpan, new GLSLOutlineTag(CURLY_TOKEN_TYPE, collapseText));
                        tokenTags.AddOutlineTagSpan(tagSpan);
                        _curlyBracketSpans.Add(tagSpan);

                        GetRootScope(span).AddChild(bracketSpan);
                    }
                }
            }

            return tokenTags;
        }

        private TokenTagCollection MatchSquareBrackets(SnapshotSpan span)
        {
            var tokenTags = new TokenTagCollection(span);
            tokenTags.SetClassifierTag(GLSLTokenTypes.SquareBracket);

            int start = span.Start;

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

                        var tagSpan = new TagSpan<GLSLOutlineTag>(bracketSpan, new GLSLOutlineTag(SQUARE_TOKEN_TYPE, collapseText));
                        tokenTags.AddOutlineTagSpan(tagSpan);
                        _squareBracketSpans.Add(tagSpan);
                    }
                }
            }

            return tokenTags;
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

            return -1;
        }
    }
}

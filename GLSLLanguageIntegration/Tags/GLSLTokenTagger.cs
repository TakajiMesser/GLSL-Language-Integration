using GLSLLanguageIntegration.Tags.Spans;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Tags
{
    internal sealed class GLSLTokenTagger : ITagger<GLSLTokenTag>
    {
        private ITextBuffer _buffer;
        //private Dictionary<string, GLSLTokenTypes> _tokenTypes = new Dictionary<string, GLSLTokenTypes>();

        internal GLSLTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            //_tokenTypes[""] = GLSLTokenTypes.;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<GLSLTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count > 0)
            {
                var position = spans.First().Start.Position;
                var endPosition = spans.Last().End.Position;
                
                var spanIndex = 0;
                var tokenIndex = 0;

                while (spanIndex < spans.Count && position < endPosition)
                {
                    var span = spans[spanIndex];

                    // Get the containing span line, and split it up into each token (delimited by space)
                    ITextSnapshotLine containingLine = span.Start.GetContainingLine();
                    string line = containingLine.GetText();
                    string[] tokens = line.Split(' ');

                    // Keep track of whether or not we get a match that consumes multiple spans
                    bool multiSpan = false;

                    // Go through each token and check it against each GLSL Span type
                    while (tokenIndex < tokens.Length && position < span.End.Position && !multiSpan)
                    {
                        var token = tokens[tokenIndex];

                        var match = FindMatch(span, spans.Subset(spanIndex + 1), token, tokens.Subset(tokenIndex + 1), position);

                        if (match.IsMatch)
                        {
                            yield return match.TagSpan;

                            // If the GLSL Span did not consume the entire first token, then we must recursively check each remaining sub-token
                            var subToken = token.Substring(0, match.Start - position);
                            foreach (var tag in GetTagsForToken(span, subToken, position))
                            {
                                yield return tag;
                            }

                            position += subToken.Length + match.Length + 1;

                            // If the match consumed past this span, increment forward to start checking the correct token
                            if (match.SpanCount > 0)
                            {
                                multiSpan = true;
                                spanIndex += match.SpanCount;
                                tokenIndex = match.TokenCount;
                            }
                            else
                            {
                                tokenIndex += match.TokenCount;
                            }
                        }
                        else
                        {
                            // If nothing matches, increment forward for the next token
                            tokenIndex++;
                            position += token.Length + 1;
                        }
                    }

                    // If nothing matches, increment forward for the next span (and reset the token index)
                    if (!multiSpan)
                    {
                        position = span.End.Position;
                        spanIndex++;
                        tokenIndex = 0;
                    }
                }
            }
        }

        private GLSLSpanMatch FindMatch(SnapshotSpan span, IEnumerable<SnapshotSpan> spans, string token, IEnumerable<string> remainingTokens, int position)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                var match = GLSLCommentSpan.Match(span, spans, token, remainingTokens, position);
                if (match.IsMatch)
                {
                    return match;
                }

                match = GLSLPreprocessorSpan.Match(span, token, remainingTokens, position);
                if (match.IsMatch)
                {
                    return match;
                }

                match = GLSLKeywordSpan.Match(span, token, position);
                if (match.IsMatch)
                {
                    return match;
                }
                
                match = GLSLTypeSpan.Match(span, token, position);
                if (match.IsMatch)
                {
                    return match;
                }
            }

            return GLSLSpanMatch.Unmatched();
        }

        private IEnumerable<ITagSpan<GLSLTokenTag>> GetTagsForToken(SnapshotSpan span, string token, int position)
        {
            if (token.Length > 0)
            {
                var match = FindMatch(span, Enumerable.Empty<SnapshotSpan>(), token, Enumerable.Empty<string>(), position);

                if (match.IsMatch)
                {
                    yield return match.TagSpan;

                    // Recursively continue checking the remaining subtoken until we have checked every character
                    var subToken = token.Substring(0, match.Start - position);

                    foreach (var tag in GetTagsForToken(span, subToken, position))
                    {
                        yield return tag;
                    }
                }
            }
        }
    }
}

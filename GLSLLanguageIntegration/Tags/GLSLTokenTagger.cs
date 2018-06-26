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
            // TODO - Handle multi-line language constructs
            //GLSLTokenTypes? currentType = null;

            foreach (var span in spans)
            {
                ITextSnapshotLine containingLine = span.Start.GetContainingLine();
                int spanStart = containingLine.Start.Position;

                // Get the containing span line, and split it up into each token (delimited by space)
                string line = containingLine.GetText();
                string[] tokens = line.Split(' ');

                var tokenIndex = 0;
                var spanIndex = 0;

                // Go through each token and check it against each GLSL Span type
                for (var i = 0; i < tokens.Length && span.Start.Position + spanIndex < span.End.Position; i++)
                {
                    var token = tokens[i];

                    var match = FindMatch(span, token, tokens.Subset(i + 1), spanIndex);

                    if (match.IsMatch)
                    {
                        yield return match.TagSpan;

                        // If the GLSL Span did not consume the entire first token, then we must recursively check each remaining sub-token
                        var subToken = token.Substring(0, match.SpanStart - (span.Start.Position + spanIndex));
                        foreach (var tag in GetTagsForToken(span, subToken, spanIndex))
                        {
                            yield return tag;
                        }

                        spanIndex += subToken.Length + match.SpanLength + 1;
                        //spanIndex += (match.SpanStart - (span.Start.Position + spanIndex)) + match.SpanLength + 1;
                        i += match.TokenCount;
                    }
                    else
                    {
                        // If nothing matches, increment spanIndex forward for the next token
                        spanIndex += token.Length + 1;
                    }
                }
            }
        }

        private GLSLSpanMatch FindMatch(SnapshotSpan span, string token, IEnumerable<string> remainingTokens, int spanIndex)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                var match = GLSLCommentSpan.Match(span, token, remainingTokens, spanIndex);
                if (match.IsMatch)
                {
                    return match;
                }

                match = GLSLPreprocessorSpan.Match(span, token, remainingTokens, spanIndex);
                if (match.IsMatch)
                {
                    return match;
                }

                match = GLSLKeywordSpan.Match(span, token, remainingTokens, spanIndex);
                if (match.IsMatch)
                {
                    return match;
                }
            }

            return GLSLSpanMatch.Unmatched();
        }

        private IEnumerable<ITagSpan<GLSLTokenTag>> GetTagsForToken(SnapshotSpan span, string token, int spanIndex)
        {
            if (token.Length > 0)
            {
                var match = FindMatch(span, token, Enumerable.Empty<string>(), spanIndex);

                if (match.IsMatch)
                {
                    yield return match.TagSpan;

                    // Recursively continue checking the remaining subtoken until we have checked every character
                    var subToken = token.Substring(0, match.SpanStart - (span.Start.Position + spanIndex));
                    //var subToken = token.Substring(0, match.SpanStart - spanIndex);

                    foreach (var tag in GetTagsForToken(span, subToken, spanIndex))
                    {
                        yield return tag;
                    }
                }
            }
        }
    }
}

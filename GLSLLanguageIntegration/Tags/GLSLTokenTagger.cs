using GLSLLanguageIntegration.Tags.Spans;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLSLLanguageIntegration.Tags
{
    internal sealed class GLSLTokenTagger : ITagger<GLSLTokenTag>
    {
        private ITextBuffer _buffer;
        private TokenBuilder _tokenBuffer = new TokenBuilder();
        //private Dictionary<string, GLSLTokenTypes> _tokenTypes = new Dictionary<string, GLSLTokenTypes>();

        internal GLSLTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            //_tokenTypes[""] = GLSLTokenTypes.;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<GLSLTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            _tokenBuffer.Clear();

            foreach (var span in spans)
            {
                // Associate the buffer with this span
                _tokenBuffer.Snapshot = span.Snapshot;

                var position = span.Start.Position;
                var endPosition = span.End.Position;
                var text = span.GetText();

                for (var i = 0; i < text.Length && span.Start.Position + i <= endPosition; i++)
                {
                    var match = ProcessCharacter(text[i], span.Start.Position + i, span);

                    if (match != null)
                    {
                        yield return match.TagSpan;
                        i += match.Length - match.Token.Length;
                    }
                }
            }
        }

        private GLSLSpanMatch ProcessCharacter(char character, int position, SnapshotSpan span)
        {
            switch (character)
            {
                case var value when char.IsWhiteSpace(character):
                    if (_tokenBuffer.Length > 0)
                    {
                        string token = _tokenBuffer.ToString();
                        var tagSpan = Tokenize(token, position, span);
                        _tokenBuffer.Clear();

                        return tagSpan;
                    }
                    break;
                case '.':
                    // Need to confirm that what came before is a valid variable/identifier
                    _tokenBuffer.Clear();
                    break;
                case ';':
                    // End of statement
                    _tokenBuffer.Clear();
                    break;
                case '{':
                case '}':
                    // Keep track of brackets and see if this created a new bracket span
                    _tokenBuffer.Clear();
                    break;
                default:
                    _tokenBuffer.Append(character, position);
                    break;
            }

            return null;
        }

        private GLSLSpanMatch Tokenize(string token, int position, SnapshotSpan span)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                var match = GLSLCommentSpan.Match(token, position, span);
                if (match.IsMatch)
                {
                    return match;
                }

                match = GLSLPreprocessorSpan.Match(token, position, span);
                if (match.IsMatch)
                {
                    return match;
                }

                match = GLSLKeywordSpan.Match(token, position, span);
                if (match.IsMatch)
                {
                    return match;
                }

                match = GLSLTypeSpan.Match(token, position, span);
                if (match.IsMatch)
                {
                    return match;
                }
            }

            return null;
        }

        /*private IEnumerable<ITagSpan<GLSLTokenTag>> GetTagsForToken(SnapshotSpan span, string token, int position)
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
        }*/
    }
}

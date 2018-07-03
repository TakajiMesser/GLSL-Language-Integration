using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Taggers;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Tags
{
    internal sealed class GLSLTokenTagger : ITagger<IGLSLTag>
    {
        private ITextBuffer _buffer;
        private TokenBuilder _tokenBuffer = new TokenBuilder();
        private GLSLTagSpanCollection _tagSpans = new GLSLTagSpanCollection();
        //private List<TagSpan<IGLSLTag>> _tagSpans = new List<TagSpan<IGLSLTag>>();

        private GLSLCommentTagger _commentTagger = new GLSLCommentTagger();
        private GLSLPreprocessorTagger _preprocessorTagger = new GLSLPreprocessorTagger();
        private GLSLKeywordTagger _keywordTagger = new GLSLKeywordTagger();
        private GLSLTypeTagger _typeTagger = new GLSLTypeTagger();

        internal GLSLTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _buffer.Changed += (s, args) =>
            {
                if (args.After == buffer.CurrentSnapshot)
                {
                    ParseBuffer();
                }
            };

            _tagSpans.TagsChanged += (s, args) => TagsChanged?.Invoke(this, args);

            ParseBuffer();
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public string GetQuickInfo(string token, GLSLTokenTypes tokenType)
        {
            switch (tokenType)
            {
                case GLSLTokenTypes.Keyword:
                    return _keywordTagger.GetQuickInfo(token);
                case GLSLTokenTypes.Type:
                    return _typeTagger.GetQuickInfo(token);
            }

            return null;
        }

        public void ParseBuffer()
        {
            var textSnapshot = _buffer.CurrentSnapshot;

            _tokenBuffer.Clear();
            _tokenBuffer.Snapshot = textSnapshot;

            var text = textSnapshot.GetText();

            var newTagSpans = new List<TagSpan<IGLSLTag>>();

            for (var i = 0; i < text.Length; i++)
            {
                foreach (var result in ProcessCharacter(text[i], i))//new SnapshotSpan(newSnapshot, i, text.Length - i)))
                {
                    newTagSpans.AddRange(result.TagSpans);
                    i += result.Consumed;
                }
            }

            _tagSpans.Update(textSnapshot, newTagSpans);
        }

        public IEnumerable<ITagSpan<IGLSLTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            _tokenBuffer.Clear();
            _tokenBuffer.Snapshot = _buffer.CurrentSnapshot;

            if (spans.Count > 0)
            {
                foreach (var tagSpan in _tagSpans.GetOverlapping(spans, _buffer.CurrentSnapshot))
                {
                    yield return tagSpan;
                }
            }

            yield break;

            foreach (var span in spans)
            {
                // Associate the buffer with this span
                //_tokenBuffer.Snapshot = span.Snapshot;

                var text = span.GetText();

                for (var i = 0; i < text.Length && span.Start.Position + i <= span.End.Position; i++)
                {
                    foreach (var result in ProcessCharacter(text[i], span.Start.Position + i/*, span*/))
                    {
                        foreach (var tagSpan in result.TagSpans)
                        {
                            yield return tagSpan;
                        }

                        i += result.Consumed;
                    }
                }
            }
        }

        private IEnumerable<GLSLSpanResult> ProcessCharacter(char character, int position)
        {
            switch (character)
            {
                case var value when char.IsWhiteSpace(character):
                    yield return ProcessBuffer(position);
                    break;
                case '.':
                    // Need to confirm that what came before is a valid variable/identifier
                    yield return ProcessBuffer(position);
                    break;
                case ';':
                    // End of statement -> Need to check statement for errors
                    yield return ProcessBuffer(position);
                    break;
                case '{':
                case '}':
                    yield return ProcessBuffer(position);
                    //yield return GLSLBracketSpan.Match(character, position, span);
                    break;
                default:
                    _tokenBuffer.Append(character, position);
                    break;
            }
        }

        private GLSLSpanResult ProcessBuffer(int position)
        {
            var result = new GLSLSpanResult();

            if (_tokenBuffer.Length > 0)
            {
                string token = _tokenBuffer.ToString();
                result = Tokenize(token, position, _tokenBuffer.Span);
                _tokenBuffer.Clear();
            }

            return result;
        }

        private GLSLSpanResult Tokenize(string token, int position, SnapshotSpan span)
        {
            var result = new GLSLSpanResult();

            if (!string.IsNullOrWhiteSpace(token))
            {
                result = _commentTagger.Match(token, position, span);

                if (!result.IsMatch)
                {
                    result = _preprocessorTagger.Match(token, position, span);
                }

                if (!result.IsMatch)
                {
                    result = _keywordTagger.Match(token, position, span);
                }

                if (!result.IsMatch)
                {
                    result = _typeTagger.Match(token, position, span);
                }
            }

            return result;
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

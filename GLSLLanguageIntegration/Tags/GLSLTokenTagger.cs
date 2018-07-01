using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tags.Spans;
using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLSLLanguageIntegration.Tags
{
    internal sealed class GLSLTokenTagger : ITagger<IGLSLTag>
    {
        private ITextBuffer _buffer;
        private TokenBuilder _tokenBuffer = new TokenBuilder();
        private List<TagSpan<IGLSLTag>> _tags = new List<TagSpan<IGLSLTag>>();
        //private Dictionary<string, GLSLTokenTypes> _tokenTypes = new Dictionary<string, GLSLTokenTypes>();

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
                    //ParseBuffer();
                }
            };

            //_tokenTypes[""] = GLSLTokenTypes.;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public void ParseBuffer()
        {
            var newSnapshot = _buffer.CurrentSnapshot;

            _tokenBuffer.Clear();
            _tokenBuffer.Snapshot = newSnapshot;

            var text = newSnapshot.GetText();

            var newTagSpans = new List<TagSpan<IGLSLTag>>();

            for (var i = 0; i < text.Length; i++)
            {
                foreach (var result in ProcessCharacter(text[i], i, new SnapshotSpan(newSnapshot, i, text.Length - i)))
                {
                    newTagSpans.AddRange(result.TagSpans);
                    i += result.Consumed;
                }
            }

            var oldSpans = _tags.Select(t => t.Span.TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive).Span);
            var newSpans = newTagSpans.Select(t => t.Span.Span);
            var removedSpans = NormalizedSpanCollection.Difference(new NormalizedSpanCollection(oldSpans), new NormalizedSpanCollection(newSpans));

            int changeStart = int.MaxValue;
            int changeEnd = -1;

            if (removedSpans.Count > 0)
            {
                changeStart = removedSpans.First().Start;
                changeEnd = removedSpans.Last().End;
            }

            if (newSpans.Count() > 0)
            {
                changeStart = Math.Min(changeStart, newSpans.First().Start);
                changeEnd = Math.Max(changeEnd, newSpans.Last().End);
            }

            _tags = newTagSpans.ToList();

            if (changeStart <= changeEnd)
            {
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(newSnapshot, Span.FromBounds(changeStart, changeEnd))));
            }
        }

        public IEnumerable<ITagSpan<IGLSLTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            _tokenBuffer.Clear();
            _tokenBuffer.Snapshot = _buffer.CurrentSnapshot;

            foreach (var span in spans)
            {
                // Associate the buffer with this span
                //_tokenBuffer.Snapshot = span.Snapshot;

                var text = span.GetText();

                for (var i = 0; i < text.Length && span.Start.Position + i <= span.End.Position; i++)
                {
                    foreach (var result in ProcessCharacter(text[i], span.Start.Position + i, span))
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

        private IEnumerable<GLSLSpanResult> ProcessCharacter(char character, int position, SnapshotSpan span)
        {
            switch (character)
            {
                case var value when char.IsWhiteSpace(character):
                    yield return ProcessBuffer(position, span);
                    break;
                case '.':
                    // Need to confirm that what came before is a valid variable/identifier
                    yield return ProcessBuffer(position, span);
                    break;
                case ';':
                    // End of statement -> Need to check statement for errors
                    yield return ProcessBuffer(position, span);
                    break;
                case '{':
                case '}':
                    yield return ProcessBuffer(position, span);
                    yield return GLSLBracketSpan.Match(character, position, span);
                    break;
                default:
                    _tokenBuffer.Append(character, position);
                    break;
            }
        }

        private GLSLSpanResult ProcessBuffer(int position, SnapshotSpan span)
        {
            var result = new GLSLSpanResult();

            if (_tokenBuffer.Length > 0)
            {
                string token = _tokenBuffer.ToString();
                result = Tokenize(token, position, span);
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

using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Outlining
{
    internal sealed class GLSLHighlighter : ITagger<GLSLHighlightTag>
    {
        private ITextView _view;
        private ITextBuffer _buffer;
        private ITextSearchService _textSearchService;
        private ITextStructureNavigator _textStructureNavigator;
        private ITagAggregator<IGLSLTag> _aggregator;

        private NormalizedSnapshotSpanCollection _wordSpans = new NormalizedSnapshotSpanCollection();
        private SnapshotSpan? _currentToken = null;
        private SnapshotPoint _requestedPoint;
        private object _updateLock = new object();

        private NormalizedSnapshotSpanCollection _tokenSpans;
        private NormalizedSnapshotSpanCollection _commentSpans;
        private List<SnapshotSpan> _parenthesisSpans = new List<SnapshotSpan>();
        private List<SnapshotSpan> _curlyBracketSpans = new List<SnapshotSpan>();
        private List<SnapshotSpan> _squareBracketSpans = new List<SnapshotSpan>();
        private object _spanLock = new object();

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        internal GLSLHighlighter(ITextView view, ITextBuffer sourceBuffer, ITextSearchService textSearchService, ITextStructureNavigator textStructureNavigator, ITagAggregator<IGLSLTag> aggregator)
        {
            _view = view;
            _buffer = sourceBuffer;
            _textSearchService = textSearchService;
            _textStructureNavigator = textStructureNavigator;
            _aggregator = aggregator;

            _aggregator.TagsChanged += (s, args) =>
            {
                var spans = args.Span.GetSpans(_buffer);
                foreach (var span in spans)
                {
                    TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
                }
            };

            _view.LayoutChanged += (s, args) =>
            {
                if (args.NewSnapshot != args.OldSnapshot)
                {
                    UpdateAtCaretPosition(_view.Caret.Position);
                }
            };
            _view.Caret.PositionChanged += (s, args) => UpdateAtCaretPosition(args.NewPosition);
        }

        public IEnumerable<ITagSpan<GLSLHighlightTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (_currentToken.HasValue)
            {
                UpdateTrackedSpans(spans);

                // Hold on to a "snapshot" of the word spans and current word, so that we maintain the same collection throughout  
                SnapshotSpan currentToken = _currentToken.Value;
                NormalizedSnapshotSpanCollection wordSpans = _wordSpans;

                if (spans.Count > 0 && wordSpans.Count > 0)
                {
                    // If the requested snapshot isn't the same as the one our words are on, translate our spans to the expected snapshot   
                    if (spans.First().Snapshot != wordSpans.First().Snapshot)
                    {
                        wordSpans = new NormalizedSnapshotSpanCollection(wordSpans.Select(s => s.TranslateTo(spans.First().Snapshot, SpanTrackingMode.EdgeExclusive)));
                        currentToken = currentToken.TranslateTo(spans.First().Snapshot, SpanTrackingMode.EdgeExclusive);
                    }

                    // First, yield back the word the cursor is under (if it overlaps)
                    // Note that we'll yield back the same word again in the wordspans collection; the duplication here is expected.   
                    yield return new TagSpan<GLSLHighlightTag>(currentToken, new GLSLHighlightTag());

                    foreach (var span in wordSpans)
                    {
                        yield return new TagSpan<GLSLHighlightTag>(span, new GLSLHighlightTag());
                    }
                }
            }
        }

        private void UpdateTrackedSpans(NormalizedSnapshotSpanCollection spans)
        {
            var textSnapshot = spans.First().Snapshot;
            var tags = _aggregator.GetTags(textSnapshot.FullSpan());

            lock (_spanLock)
            {
                _tokenSpans = new NormalizedSnapshotSpanCollection(tags.Where(t => t.Tag.TokenType.IsVariable() || t.Tag.TokenType.IsFunction())
                    .SelectMany(t => t.Span.GetSpans(textSnapshot)));

                _commentSpans = new NormalizedSnapshotSpanCollection(tags.Where(t => t.Tag.TokenType == GLSLTokenTypes.Comment)
                    .SelectMany(t => t.Span.GetSpans(textSnapshot)));

                _parenthesisSpans = new List<SnapshotSpan>(tags.Where(t => t.Tag.TokenType == GLSLTokenTypes.Parenthesis)
                    .SelectMany(t => t.Span.GetSpans(textSnapshot)));

                _curlyBracketSpans = new List<SnapshotSpan>(tags.Where(t => t.Tag.TokenType == GLSLTokenTypes.CurlyBracket)
                    .SelectMany(t => t.Span.GetSpans(textSnapshot)));

                _squareBracketSpans = new List<SnapshotSpan>(tags.Where(t => t.Tag.TokenType == GLSLTokenTypes.SquareBracket)
                    .SelectMany(t => t.Span.GetSpans(textSnapshot)));
            }
        }

        private void UpdateAtCaretPosition(CaretPosition position)
        {
            var point = position.Point.GetPoint(_buffer, position.Affinity);

            if (point.HasValue)
            {
                // If the new caret position is still within the current word (and on the same snapshot), we don't need to check it   
                if (!_currentToken.HasValue || _currentToken.Value.Snapshot != _view.TextSnapshot
                    || point.Value < _currentToken.Value.Start || point.Value > _currentToken.Value.End)
                {
                    _requestedPoint = point.Value;
                    UpdateWordAdornments();
                }
            }
        }

        private void UpdateWordAdornments()
        {
            var point = _requestedPoint;

            // Find all words in the buffer like the one the caret is on  
            TextExtent extent = _textStructureNavigator.GetExtentOfWord(point);

            // If we've selected something not worth highlighting, we might have missed a "word" by a little bit  
            if (!IsExtentValid(extent, point))
            {
                //Before we retry, make sure it is worthwhile
                if (extent.Span.Start != point || point == point.GetContainingLine().Start || char.IsWhiteSpace((point - 1).GetChar()))
                {
                    // If we couldn't find a word, clear out the existing markers  
                    SynchronousUpdate(point, new NormalizedSnapshotSpanCollection(), null);
                    return;
                }
                else
                {
                    // Try again, one character previous. If the caret is at the end of a word, pick up the word.  
                    extent = _textStructureNavigator.GetExtentOfWord(point - 1);

                    // If the word still isn't valid, we're done   
                    if (!IsExtentValid(extent, point))
                    {
                        // If we couldn't find a word, clear out the existing markers  
                        SynchronousUpdate(point, new NormalizedSnapshotSpanCollection(), null);
                        return;
                    }
                }
            }

            FindAndUpdateSpans(extent, point);
        }

        private void FindAndUpdateSpans(TextExtent extent, SnapshotPoint request)
        {
            var wordSpans = new List<SnapshotSpan>();
            var text = request.Snapshot.GetText(extent.Span);

            if (!IsComment(extent))
            {
                if (text.Any(c => char.IsLetter(c)))
                {
                    if (!_currentToken.HasValue || extent.Span != _currentToken)
                    {
                        // Find the new spans
                        var findData = new FindData(extent.Span.GetText(), extent.Span.Snapshot)
                        {
                            FindOptions = FindOptions.WholeWord | FindOptions.MatchCase
                        };

                        var spans = _textSearchService.FindAll(findData);

                        foreach (var span in spans)
                        {
                            lock (_spanLock)
                            {
                                if (!_commentSpans.CloneAndTrackTo(extent.Span.Snapshot, SpanTrackingMode.EdgePositive).OverlapsWith(span)
                                    && _tokenSpans.CloneAndTrackTo(extent.Span.Snapshot, SpanTrackingMode.EdgePositive).OverlapsWith(span))
                                {
                                    wordSpans.Add(span);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var span = GetMatchingBracketSpan(extent, text);

                    if (span.HasValue && (!_currentToken.HasValue || extent.Span != _currentToken))
                    {
                        wordSpans.Add(span.Value);
                    }
                }
            }

            // If another change hasn't happened, do a real update   
            if (request == _requestedPoint)
            {
                SynchronousUpdate(request, new NormalizedSnapshotSpanCollection(wordSpans), extent.Span);
            }
        }

        private bool IsComment(TextExtent extent)
        {
            lock (_spanLock)
            {
                // Assume everything is a comment until we properly parse
                if (_commentSpans == null)
                {
                    return true;
                }
                else
                {
                    foreach (var commentSpan in _commentSpans)
                    {
                        var translatedSpan = commentSpan.Translated(extent.Span.Snapshot);

                        if (translatedSpan.OverlapsWith(extent.Span))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private SnapshotSpan? GetMatchingBracketSpan(TextExtent extent, string text)
        {
            switch (text)
            {
                case "(":
                    var endParanthesisSpan = _parenthesisSpans.Select(s => (SnapshotSpan?)s.Translated(extent.Span.Snapshot))
                        .FirstOrDefault(p => extent.Span.OverlapsWith(new SnapshotSpan(p.Value.Start, p.Value.Start + 1)));

                    return endParanthesisSpan.HasValue
                        ? new SnapshotSpan(endParanthesisSpan.Value.End - 1, endParanthesisSpan.Value.End)
                        : (SnapshotSpan?)null;
                case ")":
                    var startParanthesisSpan = _parenthesisSpans.Select(s => (SnapshotSpan?)s.Translated(extent.Span.Snapshot))
                        .FirstOrDefault(p => extent.Span.OverlapsWith(new SnapshotSpan(p.Value.End - 1, p.Value.End)));

                    return startParanthesisSpan.HasValue
                        ? new SnapshotSpan(startParanthesisSpan.Value.Start, startParanthesisSpan.Value.Start + 1)
                        : (SnapshotSpan?)null;
                case "{":
                    var endCurlyBracketSpan = _curlyBracketSpans.Select(s => (SnapshotSpan?)s.Translated(extent.Span.Snapshot))
                        .FirstOrDefault(p => extent.Span.OverlapsWith(new SnapshotSpan(p.Value.Start, p.Value.Start + 1)));

                    return endCurlyBracketSpan.HasValue
                        ? new SnapshotSpan(endCurlyBracketSpan.Value.End - 1, endCurlyBracketSpan.Value.End)
                        : (SnapshotSpan?)null;
                case "}":
                    var startCurlyBracketSpan = _curlyBracketSpans.Select(s => (SnapshotSpan?)s.Translated(extent.Span.Snapshot))
                        .FirstOrDefault(p => extent.Span.OverlapsWith(new SnapshotSpan(p.Value.End - 1, p.Value.End)));

                    return startCurlyBracketSpan.HasValue
                        ? new SnapshotSpan(startCurlyBracketSpan.Value.Start, startCurlyBracketSpan.Value.Start + 1)
                        : (SnapshotSpan?)null;
                case "[":
                    var endSquareBracketSpan = _squareBracketSpans.Select(s => (SnapshotSpan?)s.Translated(extent.Span.Snapshot))
                        .FirstOrDefault(p => extent.Span.OverlapsWith(new SnapshotSpan(p.Value.Start, p.Value.Start + 1)));

                    return endSquareBracketSpan.HasValue
                        ? new SnapshotSpan(endSquareBracketSpan.Value.End - 1, endSquareBracketSpan.Value.End)
                        : (SnapshotSpan?)null;
                case "]":
                    var startSquareBracketSpan = _squareBracketSpans.Select(s => (SnapshotSpan?)s.Translated(extent.Span.Snapshot))
                        .FirstOrDefault(p => extent.Span.OverlapsWith(new SnapshotSpan(p.Value.End - 1, p.Value.End)));

                    return startSquareBracketSpan.HasValue
                        ? new SnapshotSpan(startSquareBracketSpan.Value.Start, startSquareBracketSpan.Value.Start + 1)
                        : (SnapshotSpan?)null;
            }

            return null;
        }

        private bool IsExtentValid(TextExtent extent, SnapshotPoint request)
        {
            var text = request.Snapshot.GetText(extent.Span);
            if (!string.IsNullOrWhiteSpace(text))//extent.IsSignificant)
            {
                return text.Any(c => char.IsLetter(c))
                    || text == "(" || text == ")"
                    || text == "{" || text == "}"
                    || text == "[" || text == "]";
            }

            return false;
        }

        private void SynchronousUpdate(SnapshotPoint request, NormalizedSnapshotSpanCollection wordSpans, SnapshotSpan? token)
        {
            lock (_updateLock)
            {
                if (request == _requestedPoint)
                {
                    _wordSpans = wordSpans;
                    _currentToken = token;

                    TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
                }
            }
        }
    }
}

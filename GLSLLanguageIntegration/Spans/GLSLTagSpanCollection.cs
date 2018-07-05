using GLSLLanguageIntegration.Tags;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Spans
{
    public class GLSLTagSpanCollection
    {
        private List<TagSpan<IGLSLTag>> _tagSpans = new List<TagSpan<IGLSLTag>>();

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public void Update(ITextSnapshot textSnapshot, IEnumerable<TagSpan<IGLSLTag>> tagSpans)
        {
            var oldSpans = _tagSpans.Select(t => t.Span.TranslateTo(textSnapshot, SpanTrackingMode.EdgeExclusive).Span);
            var newSpans = tagSpans.Select(t => t.Span.Span);
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

            _tagSpans = tagSpans.ToList();

            if (changeStart <= changeEnd)
            {
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(textSnapshot, Span.FromBounds(changeStart, changeEnd))));
            }
        }

        public IEnumerable<TagSpan<IGLSLTag>> GetOverlapping(NormalizedSnapshotSpanCollection spans, ITextSnapshot textSnapshot)
        {
            var fullSpan = new SnapshotSpan(spans.First().Start, spans.Last().End);

            foreach (var tagSpan in _tagSpans)
            {
                var translatedSpan = tagSpan.Span.TranslateTo(textSnapshot, SpanTrackingMode.EdgeExclusive);

                if (translatedSpan.IntersectsWith(fullSpan))
                {
                    // Now check more incrementally
                    foreach (var span in spans)
                    {
                        if (translatedSpan.IntersectsWith(span))
                        {
                            yield return new TagSpan<IGLSLTag>(translatedSpan, tagSpan.Tag);
                            break;
                        }
                    }
                }
            }
        }

        public IEnumerable<TagSpan<IGLSLTag>> GetOverlapping(INormalizedTextChangeCollection textChanges, ITextSnapshot textSnapshot)
        {
            var fullSpan = new Span(textChanges.First().NewSpan.Start, textChanges.Last().NewSpan.End);

            foreach (var tagSpan in _tagSpans)
            {
                var translatedSpan = tagSpan.Span.TranslateTo(textSnapshot, SpanTrackingMode.EdgeExclusive);

                if (translatedSpan.IntersectsWith(fullSpan))
                {
                    // Now check more incrementally
                    foreach (var textChange in textChanges)
                    {
                        if (translatedSpan.IntersectsWith(textChange.NewSpan))
                        {
                            yield return new TagSpan<IGLSLTag>(translatedSpan, tagSpan.Tag);
                            break;
                        }
                    }
                }
            }
        }
    }
}

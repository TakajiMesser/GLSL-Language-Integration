using GLSLLanguageIntegration.Tags;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Outlining
{
    internal sealed class GLSLOutliner : ITagger<IOutliningRegionTag>
    {
        private ITextBuffer _buffer;
        private ITextSnapshot _snapshot;
        private ITagAggregator<IGLSLTag> _aggregator;

        internal GLSLOutliner(ITextBuffer buffer, ITagAggregator<IGLSLTag> aggregator)
        {
            _buffer = buffer;
            _snapshot = buffer.CurrentSnapshot;
            _aggregator = aggregator;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var textSnapshot = spans.First().Snapshot;

            foreach (var tag in _aggregator.GetTags(spans))
            {
                if (tag.Tag is GLSLOutlineTag outlineTag)
                {
                    foreach (var span in tag.Span.GetSpans(textSnapshot))
                    {
                        // Ensure that we translate our span to the expected snapshot
                        var currentSpan = span;

                        if (currentSpan.Snapshot != textSnapshot)
                        {
                            currentSpan = span.TranslateTo(textSnapshot, SpanTrackingMode.EdgePositive);
                        }

                        yield return new TagSpan<IOutliningRegionTag>(currentSpan, outlineTag.RegionTag);
                    }
                }
            }
        }
    }
}

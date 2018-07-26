using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GLSLLanguageIntegration.Intellisense
{
    internal sealed class GLSLQuickInfoSource : IAsyncQuickInfoSource
    {
        private ITagAggregator<IGLSLTag> _aggregator;
        private ITextBuffer _buffer;
        private bool _disposed = false;

        public GLSLQuickInfoSource(ITextBuffer buffer, ITagAggregator<IGLSLTag> aggregator)
        {
            _buffer = buffer;
            _aggregator = aggregator;
        }

        public async Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(GLSLQuickInfoSource));

            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(_buffer.CurrentSnapshot);

            if (triggerPoint != null)
            {
                var tagger = new GLSLTokenTagProvider().CreateTagger<IGLSLTag>(_buffer) as GLSLTokenTagger;
                var triggerSpan = new SnapshotSpan(triggerPoint, triggerPoint);

                foreach (var tag in _aggregator.GetTags(triggerSpan))
                {
                    if (cancellationToken.IsCancellationRequested) return null;

                    // Ignore outline tags, since they will likely place our trigger point somewhere awkward
                    if (tag.Tag is GLSLClassifierTag)
                    {
                        var span = tag.Span.GetSpans(_buffer.CurrentSnapshot).First();
                        var applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                        
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
                        var quickInfo = tagger.GetQuickInfo(span.GetText(), tag.Tag.TokenType);

                        if (quickInfo != null)
                        {
                            return new QuickInfoItem(applicableToSpan, quickInfo);
                        }
                    }
                }
            }

            return null;
        }

        private QuickInfoItem GetQuickInfo(SnapshotPoint triggerPoint, CancellationToken cancellationToken)
        {
            var tagger = new GLSLTokenTagProvider().CreateTagger<IGLSLTag>(_buffer) as GLSLTokenTagger;
            var triggerSpan = new SnapshotSpan(triggerPoint, triggerPoint);

            foreach (var tag in _aggregator.GetTags(triggerSpan))
            {
                if (cancellationToken.IsCancellationRequested) return null;

                // Ignore outline tags, since they will likely place our trigger point somewhere awkward
                if (tag.Tag is GLSLClassifierTag)
                {
                    var span = tag.Span.GetSpans(_buffer.CurrentSnapshot).First();
                    var applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                    var quickInfo = tagger.GetQuickInfo(span.GetText(), tag.Tag.TokenType);

                    if (quickInfo != null)
                    {
                        return new QuickInfoItem(applicableToSpan, quickInfo);
                    }
                }
            }

            return null;
        }

        private object GetQuickInfoFromTag(IMappingTagSpan<IGLSLTag> tag, out ITrackingSpan applicableToSpan)
        {
            var span = tag.Span.GetSpans(_buffer.CurrentSnapshot).First();
            applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);

            var tagger = new GLSLTokenTagProvider().CreateTagger<IGLSLTag>(_buffer) as GLSLTokenTagger;
            var quickInfo = tagger.GetQuickInfo(span.GetText(), tag.Tag.TokenType);

            return quickInfo;
        }

        public void Dispose() => _disposed = true;
    }
}

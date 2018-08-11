using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

                var quickInfo = await GetQuickInfoAsync(tagger, triggerSpan, cancellationToken);
                if (quickInfo != null)
                {
                    return quickInfo;
                }
            }

            return null;
        }

        private async Task<QuickInfoItem> GetQuickInfoAsync(GLSLTokenTagger tagger, SnapshotSpan triggerSpan, CancellationToken cancellationToken)
        {
            foreach (var tag in _aggregator.GetTags(triggerSpan))
            {
                if (cancellationToken.IsCancellationRequested) return null;

                // Ignore outline tags, since they will likely place our trigger point somewhere awkward
                if (tag.Tag is GLSLClassifierTag)
                {
                    var span = tag.Span.GetSpans(_buffer.CurrentSnapshot).First();
                    var applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
                    var quickInfo = tagger.GetQuickInfo(span, tag.Tag.TokenType);
                    
                    if (quickInfo != null)
                    {
                        return new QuickInfoItem(applicableToSpan, quickInfo);
                    }
                }
            }

            return null;
        }

        public void Dispose() => _disposed = true;
    }
}

using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace GLSLLanguageIntegration.Intellisense
{
    internal sealed class GLSLQuickInfoSource : IQuickInfoSource
    {
        private ITagAggregator<IGLSLTag> _aggregator;
        private ITextBuffer _buffer;
        private bool _disposed = false;

        public GLSLQuickInfoSource(ITextBuffer buffer, ITagAggregator<IGLSLTag> aggregator)
        {
            _buffer = buffer;
            _aggregator = aggregator;
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;
            if (_disposed) throw new ObjectDisposedException(nameof(GLSLQuickInfoSource));

            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(_buffer.CurrentSnapshot);

            if (triggerPoint != null)
            {
                foreach (var tag in _aggregator.GetTags(new SnapshotSpan(triggerPoint, triggerPoint)))
                {
                    // Ignore outline tags, since they will likely place our trigger point somewhere awkward
                    if (tag.Tag is GLSLClassifierTag)
                    {
                        var quickInfo = GetQuickInfo(tag, out applicableToSpan);
                        if (quickInfo != null)
                        {
                            quickInfoContent.Add(quickInfo);
                            break;
                        }
                    }
                }
            }
        }

        private object GetQuickInfo(IMappingTagSpan<IGLSLTag> tag, out ITrackingSpan applicableToSpan)
        {
            var span = tag.Span.GetSpans(_buffer.CurrentSnapshot).First();
            applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);

            var tagger = new GLSLTokenTagProvider().CreateTagger<IGLSLTag>(_buffer) as GLSLTokenTagger;
            var quickInfo = tagger.GetQuickInfo(span.GetText(), tag.Tag.TokenType);

            return quickInfo;
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}

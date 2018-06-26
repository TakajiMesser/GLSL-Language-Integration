using GLSLLanguageIntegration.Tags;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLLanguageIntegration.Intellisense
{
    internal sealed class GLSLQuickInfoSource : IQuickInfoSource
    {
        private ITagAggregator<GLSLTokenTag> _aggregator;
        private ITextBuffer _buffer;
        private bool _disposed = false;

        public GLSLQuickInfoSource(ITextBuffer buffer, ITagAggregator<GLSLTokenTag> aggregator)
        {
            _buffer = buffer;
            _aggregator = aggregator;
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;

            if (_disposed) throw new ObjectDisposedException("TestQuickInfoSource");

            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(_buffer.CurrentSnapshot);

            if (triggerPoint != null)
            {
                foreach (var tag in _aggregator.GetTags(new SnapshotSpan(triggerPoint, triggerPoint)))
                {
                    switch (tag.Tag.TokenType)
                    {
                        case GLSLTokenTypes.Preprocessor:
                            var preprocessorTagSpan = tag.Span.GetSpans(_buffer).First();
                            applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(preprocessorTagSpan, SpanTrackingMode.EdgeExclusive);
                            quickInfoContent.Add("Preprocessor");
                            break;
                    }
                }
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}

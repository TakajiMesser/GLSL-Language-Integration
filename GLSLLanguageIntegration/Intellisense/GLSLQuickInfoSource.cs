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
                        case GLSLTokenTypes.Comment:
                            var commentTagSpan = tag.Span.GetSpans(_buffer).First();
                            applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(commentTagSpan, SpanTrackingMode.EdgeExclusive);
                            quickInfoContent.Add("Comment");
                            break;
                        case GLSLTokenTypes.Keyword:
                            var keywordtagSpan = tag.Span.GetSpans(_buffer).First();
                            applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(keywordtagSpan, SpanTrackingMode.EdgeExclusive);

                            var keywordTagger = new GLSLTokenTagProvider().CreateTagger<IGLSLTag>(_buffer) as GLSLTokenTagger;
                            var keywordQuickInfo = keywordTagger.GetQuickInfo(keywordtagSpan.GetText(), tag.Tag.TokenType);
                            if (!string.IsNullOrEmpty(keywordQuickInfo))
                            {
                                quickInfoContent.Add(keywordQuickInfo);
                            }
                            else
                            {
                                quickInfoContent.Add("Keyword");
                            }
                            break;
                        case GLSLTokenTypes.Type:
                            var typeTagSpan = tag.Span.GetSpans(_buffer).First();
                            applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(typeTagSpan, SpanTrackingMode.EdgeExclusive);

                            var typeTagger = new GLSLTokenTagProvider().CreateTagger<IGLSLTag>(_buffer) as GLSLTokenTagger;
                            var typeQuickInfo = typeTagger.GetQuickInfo(typeTagSpan.GetText(), tag.Tag.TokenType);
                            if (!string.IsNullOrEmpty(typeQuickInfo))
                            {
                                quickInfoContent.Add(typeQuickInfo);
                            }
                            else
                            {
                                quickInfoContent.Add("Type");
                            }
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

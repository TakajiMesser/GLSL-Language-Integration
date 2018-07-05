using GLSLLanguageIntegration.Tags;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

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
                        case GLSLTokenTypes.Keyword:
                            quickInfoContent.Add(GetQuickInfo(tag, "Keyword", out applicableToSpan));
                            break;
                        case GLSLTokenTypes.Type:
                            quickInfoContent.Add(GetQuickInfo(tag, "Type", out applicableToSpan));
                            break;
                        case GLSLTokenTypes.InputVariable:
                        case GLSLTokenTypes.OutputVariable:
                        case GLSLTokenTypes.UniformVariable:
                        case GLSLTokenTypes.BufferVariable:
                        case GLSLTokenTypes.SharedVariable:
                        case GLSLTokenTypes.BuiltInVariable:
                            quickInfoContent.Add(GetQuickInfo(tag, "Variable", out applicableToSpan));
                            break;
                        case GLSLTokenTypes.Struct:
                            break;
                        case GLSLTokenTypes.IntegerConstant:
                        case GLSLTokenTypes.FloatingConstant:
                        case GLSLTokenTypes.BuiltInConstant:
                            break;
                        case GLSLTokenTypes.Function:
                        case GLSLTokenTypes.BuiltInFunction:
                            break;
                        case GLSLTokenTypes.Operator:
                            break;
                        case GLSLTokenTypes.Semicolon:
                            break;
                        case GLSLTokenTypes.Bracket:
                            break;
                    }
                }
            }
        }

        private object GetQuickInfo(IMappingTagSpan<IGLSLTag> tag, string baseText, out ITrackingSpan applicableToSpan)
        {
            var tagSpan = tag.Span.GetSpans(_buffer).First();
            applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);

            var tagger = new GLSLTokenTagProvider().CreateTagger<IGLSLTag>(_buffer) as GLSLTokenTagger;
            var quickInfo = tagger.GetQuickInfo(tagSpan.GetText(), tag.Tag.TokenType);

            return quickInfo ?? baseText;
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}

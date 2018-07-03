using GLSLLanguageIntegration.Tags;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GLSLLanguageIntegration.Classification
{
    /*IntegerConstant,
    FloatingConstant,
    Operator,
    Semicolon*/
    internal sealed class GLSLClassifier : ITagger<ClassificationTag>
    {
        private ITextBuffer _buffer;
        private ITagAggregator<IGLSLTag> _aggregator;
        private Dictionary<GLSLTokenTypes, IClassificationType> _glslTypes;

        internal GLSLClassifier(ITextBuffer buffer, ITagAggregator<IGLSLTag> glslTagAggregator, IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;
            /*_buffer.Changed += (s, args) =>
            {
                if (args.After == buffer.CurrentSnapshot)
                {
                    _aggregator
                }
            };*/

            _aggregator = glslTagAggregator;
            _aggregator.TagsChanged += (s, args) =>
            {
                /*var spans = args.Span.GetSpans(_buffer);
                foreach (var span in spans)
                {
                    TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
                }*/
            };

            _glslTypes = new Dictionary<GLSLTokenTypes, IClassificationType>
            {
                [GLSLTokenTypes.Preprocessor] = typeService.GetClassificationType(nameof(GLSLPreprocessor)),
                [GLSLTokenTypes.Comment] = typeService.GetClassificationType(nameof(GLSLComment)),
                [GLSLTokenTypes.Keyword] = typeService.GetClassificationType(nameof(GLSLKeyword)),
                [GLSLTokenTypes.Type] = typeService.GetClassificationType(nameof(GLSLType)),
                [GLSLTokenTypes.Identifier] = typeService.GetClassificationType(nameof(GLSLIdentifier)),
                [GLSLTokenTypes.Bracket] = typeService.GetClassificationType(nameof(GLSLBracket))
            };
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var textSnapshot = spans.First().Snapshot;

            foreach (var tag in _aggregator.GetTags(spans))
            {
                if (tag.Tag is GLSLTokenTag)
                {
                    foreach (var span in tag.Span.GetSpans(textSnapshot))
                    {
                        // Ensure that we translate our span to the expected snapshot
                        var currentSpan = span;

                        if (currentSpan.Snapshot != textSnapshot)
                        {
                            currentSpan = span.TranslateTo(textSnapshot, SpanTrackingMode.EdgePositive);
                        }

                        yield return new TagSpan<ClassificationTag>(currentSpan, new ClassificationTag(_glslTypes[tag.Tag.TokenType]));
                    }
                }

                //var snapshots = tag.Span.GetSpans(spans.First().Snapshot);
                //yield return new TagSpan<ClassificationTag>(snapshots.First(), new ClassificationTag(_glslTypes[tag.Tag.TokenType]));
            }
        }
    }
}

using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Classification
{
    internal sealed class GLSLClassifier : ITagger<ClassificationTag>
    {
        private ITextBuffer _buffer;
        private ITagAggregator<IGLSLTag> _aggregator;
        private Dictionary<GLSLTokenTypes, IClassificationType> _glslTypes;
        //private GLSLTagSpanCollection _tagSpans = new GLSLTagSpanCollection();

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        internal GLSLClassifier(ITextBuffer buffer, ITagAggregator<IGLSLTag> glslTagAggregator, IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;

            _aggregator = glslTagAggregator;
            _aggregator.TagsChanged += (s, args) =>
            {
                var spans = args.Span.GetSpans(_buffer);
                foreach (var span in spans)
                {
                    TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
                }
            };

            /*_tagSpans.TagsChanged += (s, args) =>
            {
                TagsChanged?.Invoke(this, args);
            };*/

            _glslTypes = new Dictionary<GLSLTokenTypes, IClassificationType>
            {
                [GLSLTokenTypes.Preprocessor] = typeService.GetClassificationType(nameof(GLSLPreprocessor)),
                [GLSLTokenTypes.Comment] = typeService.GetClassificationType(nameof(GLSLComment)),
                [GLSLTokenTypes.Keyword] = typeService.GetClassificationType(nameof(GLSLKeyword)),
                [GLSLTokenTypes.Type] = typeService.GetClassificationType(nameof(GLSLType)),
                [GLSLTokenTypes.InputVariable] = typeService.GetClassificationType(nameof(GLSLInputVariable)),
                [GLSLTokenTypes.OutputVariable] = typeService.GetClassificationType(nameof(GLSLOutputVariable)),
                [GLSLTokenTypes.UniformVariable] = typeService.GetClassificationType(nameof(GLSLUniformVariable)),
                [GLSLTokenTypes.BufferVariable] = typeService.GetClassificationType(nameof(GLSLBufferVariable)),
                [GLSLTokenTypes.SharedVariable] = typeService.GetClassificationType(nameof(GLSLSharedVariable)),
                [GLSLTokenTypes.BuiltInVariable] = typeService.GetClassificationType(nameof(GLSLBuiltInVariable)),
                [GLSLTokenTypes.LocalVariable] = typeService.GetClassificationType(nameof(GLSLLocalVariable)),
                [GLSLTokenTypes.Struct] = typeService.GetClassificationType(nameof(GLSLStruct)),
                [GLSLTokenTypes.IntegerConstant] = typeService.GetClassificationType(nameof(GLSLIntegerConstant)),
                [GLSLTokenTypes.FloatingConstant] = typeService.GetClassificationType(nameof(GLSLFloatingConstant)),
                [GLSLTokenTypes.BuiltInConstant] = typeService.GetClassificationType(nameof(GLSLBuiltInConstant)),
                [GLSLTokenTypes.Function] = typeService.GetClassificationType(nameof(GLSLFunction)),
                [GLSLTokenTypes.BuiltInFunction] = typeService.GetClassificationType(nameof(GLSLBuiltInFunction)),
                [GLSLTokenTypes.Operator] = typeService.GetClassificationType(nameof(GLSLOperator)),
                [GLSLTokenTypes.Semicolon] = typeService.GetClassificationType(nameof(GLSLSemicolon)),
                [GLSLTokenTypes.Parenthesis] = typeService.GetClassificationType(nameof(GLSLParenthesis)),
                [GLSLTokenTypes.CurlyBracket] = typeService.GetClassificationType(nameof(GLSLCurlyBracket)),
                [GLSLTokenTypes.SquareBracket] = typeService.GetClassificationType(nameof(GLSLSquareBracket))
            };
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var textSnapshot = spans.First().Snapshot;

            foreach (var tag in _aggregator.GetTags(spans))
            {
                if (tag.Tag is GLSLClassifierTag)
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
            }
        }
    }
}

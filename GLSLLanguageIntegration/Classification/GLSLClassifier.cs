﻿using GLSLLanguageIntegration.Tags;
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
        private ITagAggregator<GLSLTokenTag> _aggregator;
        private Dictionary<GLSLTokenTypes, IClassificationType> _glslTypes;

        internal GLSLClassifier(ITextBuffer buffer, ITagAggregator<GLSLTokenTag> glslTagAggregator, IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;
            _aggregator = glslTagAggregator;

            _glslTypes = new Dictionary<GLSLTokenTypes, IClassificationType>
            {
                [GLSLTokenTypes.Preprocessor] = typeService.GetClassificationType("preprocessor"),
                [GLSLTokenTypes.Comment] = typeService.GetClassificationType("comment"),
                [GLSLTokenTypes.Keyword] = typeService.GetClassificationType("keyword"),
                [GLSLTokenTypes.Identifier] = typeService.GetClassificationType("identifier"),
                [GLSLTokenTypes.Bracket] = typeService.GetClassificationType("bracket")
            };
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var tag in _aggregator.GetTags(spans))
            {
                var tagSpans = tag.Span.GetSpans(spans[0].Snapshot);
                yield return new TagSpan<ClassificationTag>(tagSpans[0], new ClassificationTag(_glslTypes[tag.Tag.TokenType]));
            }
        }
    }
}

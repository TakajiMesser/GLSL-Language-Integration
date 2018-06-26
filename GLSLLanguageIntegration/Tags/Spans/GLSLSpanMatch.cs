using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLSLLanguageIntegration.Properties;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GLSLLanguageIntegration.Tags.Spans
{
    public class GLSLSpanMatch
    {
        public bool IsMatch { get; private set; }
        public GLSLTokenTypes TokenType { get; private set; }

        /// <summary>
        /// Index of the first token that the GLSL Span starts at
        /// </summary>
        public int SpanStart { get; private set; }

        public int SpanLength { get; private set; }

        /// <summary>
        /// Number of tokens the GLSL Span consumes
        /// </summary>
        public int TokenCount { get; private set; }

        public TagSpan<GLSLTokenTag> TagSpan { get; private set; }

        /// <summary>
        /// Use GLSLSpanMatch.Matched() or GLSLSpanMatch.Unmatched() instead
        /// </summary>
        public GLSLSpanMatch() { }

        public static GLSLSpanMatch Matched(GLSLTokenTypes tokenType, int spanStart, int spanLength, int tokenCount, SnapshotSpan span)
        {
            var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(spanStart, spanLength));

            return new GLSLSpanMatch()
            {
                IsMatch = true,
                TokenType = tokenType,
                SpanStart = spanStart,
                SpanLength = spanLength,
                TokenCount = tokenCount,
                TagSpan = tokenSpan.IntersectsWith(span) ? new TagSpan<GLSLTokenTag>(tokenSpan, new GLSLTokenTag(tokenType)) : null
            };
        }

        public static GLSLSpanMatch Unmatched() => new GLSLSpanMatch()
        {
            IsMatch = false
        };
    }
}

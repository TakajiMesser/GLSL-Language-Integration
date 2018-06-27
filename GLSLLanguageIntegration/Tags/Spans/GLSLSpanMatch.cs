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
        public int Start { get; private set; }

        public int Length { get; private set; }

        /// <summary>
        /// Number of spans the snapshot consumed
        /// </summary>
        public int SpanCount { get; private set; }

        /// <summary>
        /// Number of tokens (into the last span) this snapshot consumed
        /// </summary>
        public int TokenCount { get; private set; }

        public TagSpan<GLSLTokenTag> TagSpan { get; private set; }

        /// <summary>
        /// Use GLSLSpanMatch.Matched() or GLSLSpanMatch.Unmatched() instead
        /// </summary>
        public GLSLSpanMatch() { }

        public static GLSLSpanMatch Matched(SnapshotSpan span, GLSLTokenTypes tokenType, int start, int length, int spanCount, int tokenCount)
        {
            var tokenSpan = new SnapshotSpan(span.Snapshot, start, length);

            return new GLSLSpanMatch()
            {
                IsMatch = true,
                TokenType = tokenType,
                Start = start,
                Length = length,
                SpanCount = spanCount,
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

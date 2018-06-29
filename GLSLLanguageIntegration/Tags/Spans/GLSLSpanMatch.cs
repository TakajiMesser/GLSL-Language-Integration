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
        public string Token { get; private set; }
        public GLSLTokenTypes TokenType { get; private set; }
        public int Start { get; private set; }
        public int Length { get; private set; }

        public TagSpan<GLSLTokenTag> TagSpan { get; private set; }

        /// <summary>
        /// Use GLSLSpanMatch.Matched() or GLSLSpanMatch.Unmatched() instead
        /// </summary>
        public GLSLSpanMatch() { }

        public static GLSLSpanMatch Matched(SnapshotSpan span, string token, SnapshotSpan glslSpan, GLSLTokenTypes tokenType)
        {
            return new GLSLSpanMatch()
            {
                IsMatch = true,
                Token = token,
                TokenType = tokenType,
                Start = glslSpan.Start.Position,
                Length = glslSpan.Length,
                TagSpan = glslSpan.IntersectsWith(span) ? new TagSpan<GLSLTokenTag>(glslSpan, new GLSLTokenTag(tokenType)) : null
            };
        }

        public static GLSLSpanMatch Unmatched() => new GLSLSpanMatch()
        {
            IsMatch = false
        };
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Tags;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GLSLLanguageIntegration.Spans
{
    public class GLSLSpanResult
    {
        public GLSLTokenTypes TokenType { get; private set; }
        public SnapshotSpan Span { get; private set; }

        public int Consumed { get; set; }

        public List<TagSpan<IGLSLTag>> TagSpans { get; private set; } = new List<TagSpan<IGLSLTag>>();
        public bool IsMatch => TagSpans.Count > 0;

        public GLSLSpanResult() { }
        public GLSLSpanResult(GLSLTokenTypes tokenType, SnapshotSpan span)
        {
            TokenType = tokenType;
            Span = span;
        }

        public void AddSpan(TagSpan<IGLSLTag> tagSpan)
        {
            if (Span.OverlapsWith(tagSpan.Span))
            {
                TagSpans.Add(tagSpan);
            }
        }

        public void AddSpan<T>(SnapshotSpan span) where T : IGLSLTag
        {
            if (Span.OverlapsWith(span))
            {
                if (typeof(T) == typeof(GLSLTokenTag))
                {
                    TagSpans.Add(new TagSpan<IGLSLTag>(span, new GLSLTokenTag(TokenType)));
                }
                else if (typeof(T) == typeof(GLSLOutlineTag))
                {
                    TagSpans.Add(new TagSpan<IGLSLTag>(span, new GLSLOutlineTag(TokenType)));
                }
            }
        }

        public void AddSpans(IEnumerable<TagSpan<IGLSLTag>> tagSpans)
        {
            foreach (var tagSpan in tagSpans)
            {
                if (Span.OverlapsWith(tagSpan.Span))
                {
                    TagSpans.Add(tagSpan);
                }
            }
        }

        public void AddSpans<T>(IEnumerable<SnapshotSpan> spans) where T : IGLSLTag
        {
            foreach (var span in spans)
            {
                if (Span.OverlapsWith(span))
                {
                    if (typeof(T) == typeof(GLSLTokenTag))
                    {
                        TagSpans.Add(new TagSpan<IGLSLTag>(span, new GLSLTokenTag(TokenType)));
                    }
                    else if (typeof(T) == typeof(GLSLOutlineTag))
                    {
                        TagSpans.Add(new TagSpan<IGLSLTag>(span, new GLSLOutlineTag(TokenType)));
                    }
                }
            }
        }
    }
}

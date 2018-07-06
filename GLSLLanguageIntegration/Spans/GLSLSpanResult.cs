using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Outlining;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;

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
            Consumed = 0;
        }

        public void AddSpan(TagSpan<IGLSLTag> tagSpan)
        {
            var translatedSpan = tagSpan.Span.TranslateTo(Span.Snapshot, SpanTrackingMode.EdgeExclusive);

            if (Span.OverlapsWith(translatedSpan))
            {
                TagSpans.Add(new TagSpan<IGLSLTag>(translatedSpan, tagSpan.Tag));
            }
        }

        public void AddSpan<T>(SnapshotSpan span) where T : IGLSLTag
        {
            var translatedSpan = span.TranslateTo(Span.Snapshot, SpanTrackingMode.EdgeExclusive);

            if (Span.OverlapsWith(translatedSpan))
            {
                if (typeof(T) == typeof(GLSLClassifierTag))
                {
                    TagSpans.Add(new TagSpan<IGLSLTag>(translatedSpan, new GLSLClassifierTag(TokenType)));
                }
                else if (typeof(T) == typeof(GLSLOutlineTag))
                {
                    TagSpans.Add(new TagSpan<IGLSLTag>(translatedSpan, new GLSLOutlineTag(TokenType)));
                }
            }
        }

        public void AddSpans(IEnumerable<TagSpan<IGLSLTag>> tagSpans)
        {
            foreach (var tagSpan in tagSpans)
            {
                var translatedSpan = tagSpan.Span.TranslateTo(Span.Snapshot, SpanTrackingMode.EdgeExclusive);

                if (Span.OverlapsWith(translatedSpan))
                {
                    TagSpans.Add(new TagSpan<IGLSLTag>(translatedSpan, tagSpan.Tag));
                }
            }
        }

        public void AddSpans<T>(IEnumerable<SnapshotSpan> spans) where T : IGLSLTag
        {
            foreach (var span in spans)
            {
                var translatedSpan = span.TranslateTo(Span.Snapshot, SpanTrackingMode.EdgeExclusive);

                if (Span.OverlapsWith(translatedSpan))
                {
                    if (typeof(T) == typeof(GLSLClassifierTag))
                    {
                        TagSpans.Add(new TagSpan<IGLSLTag>(translatedSpan, new GLSLClassifierTag(TokenType)));
                    }
                    else if (typeof(T) == typeof(GLSLOutlineTag))
                    {
                        TagSpans.Add(new TagSpan<IGLSLTag>(translatedSpan, new GLSLOutlineTag(TokenType)));
                    }
                }
            }
        }
    }
}

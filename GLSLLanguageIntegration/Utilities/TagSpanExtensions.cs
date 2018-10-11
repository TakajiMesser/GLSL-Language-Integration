using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace GLSLLanguageIntegration.Utilities
{
    public static class TagSpanExtensions
    {
        public static void Translate<T>(this TagSpan<T> tagSpan, ITextSnapshot snapshot) where T : ITag
        {
            tagSpan = new TagSpan<T>(tagSpan.Span.TranslateTo(snapshot, SpanTrackingMode.EdgeExclusive), tagSpan.Tag);
        }
    }
}

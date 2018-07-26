using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Utilities
{
    public static class SnapshotExtensions
    {
        public static SnapshotSpan Translated(this SnapshotSpan span, ITextSnapshot snapshot) => span.Snapshot != snapshot
            ? span.TranslateTo(snapshot, SpanTrackingMode.EdgePositive)
            : span;
    }
}

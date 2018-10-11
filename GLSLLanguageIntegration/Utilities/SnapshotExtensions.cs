using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Utilities
{
    public static class SnapshotExtensions
    {
        public static SnapshotSpan Translated(this SnapshotSpan span, ITextSnapshot snapshot) => span.Snapshot != snapshot
            ? span.TranslateTo(snapshot, SpanTrackingMode.EdgePositive)
            : span;

        public static SnapshotSpan FullSpan(this ITextSnapshot snapshot) => new SnapshotSpan(snapshot, 0, snapshot.Length);

        public static SnapshotSpan Extended(this SnapshotSpan span, int amount) =>
            new SnapshotSpan(span.Snapshot, span.Start, span.Length + amount);

        public static SnapshotSpan Prepended(this SnapshotSpan span, int amount) =>
            new SnapshotSpan(span.Snapshot, span.Start - amount, span.Length + amount);

        public static SnapshotSpan Shifted(this SnapshotSpan span, int amount) =>
            new SnapshotSpan(span.Snapshot, span.Start + amount, span.Length + amount);
    }
}

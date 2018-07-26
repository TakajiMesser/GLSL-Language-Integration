using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using System;

namespace GLSLLanguageIntegration.Utilities
{
    public static class SnapshotExtensions
    {
        public static SnapshotSpan Translated(this SnapshotSpan span, ITextSnapshot snapshot) => span.Snapshot != snapshot
            ? span.TranslateTo(snapshot, SpanTrackingMode.EdgePositive)
            : span;
    }
}

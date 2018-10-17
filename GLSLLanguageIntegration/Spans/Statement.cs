using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Spans
{
    public class Statement
    {
        public SnapshotSpan Span { get; private set; }
        public List<TagSpan<IGLSLTag>> TagSpans { get; } = new List<TagSpan<IGLSLTag>>();
        public List<Statement> ChildStatements { get; } = new List<Statement>();

        public Statement(SnapshotSpan span)
        {
            Span = span;
        }

        public void Prepend(int amount)
        {
            Span = Span.Prepended(amount);
        }

        public void Extend(int amount)
        {
            Span = Span.Extended(amount);
        }

        public void Shift(int amount)
        {
            Span = Span.Shifted(amount);
        }

        public void Translate(ITextSnapshot textSnapshot)
        {
            Span = Span.Translated(SpanTrackingMode.EdgeExclusive, textSnapshot);

            for (var i = 0; i < TagSpans.Count; i++)
            {
                var tagSpan = TagSpans[i];
                TagSpans[i] = tagSpan.Translated(textSnapshot);
            }
        }

        /// <summary>
        /// Purge any TagSpans that intersect in any way with the text changes, then update all spans
        /// </summary>
        public void PurgeAndUpdate(INormalizedTextChangeCollection textChanges, ITextSnapshot textSnapshot)
        {
            Span = Span.Translated(SpanTrackingMode.EdgeExclusive, textSnapshot);

            var fullChangeSpan = new Span(textChanges.First().NewSpan.Start, textChanges.Last().NewSpan.End);

            // Purge ALL TagSpans from any statements that intersect with these text changes
            if (Span.IntersectsWith(fullChangeSpan))
            {
                // Now check more incrementally
                if (textChanges.Any(t => Span.IntersectsWith(t.NewSpan)))
                {
                    TagSpans.Clear();
                }
            }

            /*for (var i = TagSpans.Count - 1; i >= 0; i--)
            {
                var tagSpan = TagSpans[i];
                tagSpan.Translate(textSnapshot);

                if (tagSpan.Span.IntersectsWith(fullChangeSpan))
                {
                    // Now check more incrementally
                    if (textChanges.Any(t => tagSpan.Span.IntersectsWith(t.NewSpan)))
                    {
                        // Remove this tagspan!
                        TagSpans.RemoveAt(i);
                    }
                }
            }*/
        }
    }
}

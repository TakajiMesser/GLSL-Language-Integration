using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;

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
    }
}

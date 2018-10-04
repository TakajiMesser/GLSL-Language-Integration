using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Spans
{
    public class Statement
    {
        public SnapshotSpan Span { get; }
        public List<TagSpan<IGLSLTag>> TagSpans { get; } = new List<TagSpan<IGLSLTag>>();
        public List<Statement> ChildStatements { get; } = new List<Statement>();

        public Statement(SnapshotSpan span)
        {
            Span = span;
        }
    }
}

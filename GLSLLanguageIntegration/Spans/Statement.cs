using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Spans
{
    public class Statement
    {
        public SnapshotSpan Span { get; set; }

        public Statement(SnapshotSpan span)
        {
            Span = span;
        }
    }
}

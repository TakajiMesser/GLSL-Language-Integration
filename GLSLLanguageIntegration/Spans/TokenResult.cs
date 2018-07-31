using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Spans
{
    public class TokenResult
    {
        public GLSLTokenTypes? TokenType { get; set; }
        public SnapshotSpan Span { get; set; }
        public bool IsMatch => TokenType.HasValue;
        public string Token => Span.GetText();

        public TokenResult(SnapshotSpan span)
        {
            Span = span;
        }
    }
}

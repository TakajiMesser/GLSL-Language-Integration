using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLStatementTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Semicolon;

        public SpanResult Match(SnapshotSpan span)
        {
            var result = new SpanResult(TOKEN_TYPE, span);
            return result;
        }

        public void Clear() { }
    }
}

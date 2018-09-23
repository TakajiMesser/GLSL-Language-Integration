using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLStatementTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Semicolon;

        public TokenTagCollection Match(SnapshotSpan span)
        {
            return new TokenTagCollection(span);
        }

        public void Clear() { }
    }
}

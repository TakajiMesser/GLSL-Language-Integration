using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLOperatorTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Operator;

        public TokenTagCollection Match(SnapshotSpan span)
        {
            return new TokenTagCollection(span);
        }

        public void Clear() { }
    }
}

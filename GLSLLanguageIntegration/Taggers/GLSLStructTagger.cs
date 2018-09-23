using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLStructTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Struct;

        public TokenTagCollection Match(SnapshotSpan span)
        {
            return new TokenTagCollection(span);
        }

        public void Clear() { }
    }
}

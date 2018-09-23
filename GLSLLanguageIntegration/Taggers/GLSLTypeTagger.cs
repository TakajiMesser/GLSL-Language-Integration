using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLTypeTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Type;

        private TokenSet _tokens = new TokenSet(Resources.Types, TOKEN_TYPE);

        public object GetQuickInfo(string token) => _tokens.Contains(token)? _tokens.GetInfo(token).ToQuickInfo() : "Type";

        public TokenTagCollection Match(SnapshotSpan span)
        {
            var tokenTags = new TokenTagCollection(span);

            string token = span.GetText();

            if (_tokens.Contains(token))
            {
                tokenTags.SetClassifierTag(TOKEN_TYPE);
            }

            return tokenTags;
        }

        public void Clear() { }
    }
}

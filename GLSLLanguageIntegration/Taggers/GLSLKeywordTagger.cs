using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLKeywordTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Keyword;

        private static TokenSet _tokens = new TokenSet(Resources.Keywords, TOKEN_TYPE);

        public object GetQuickInfo(string token) => _tokens.Contains(token) ? _tokens.GetInfo(token).ToQuickInfo() : null;

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

using GLSLLanguageIntegration.Classification;
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

        public SpanResult Match(SnapshotSpan span)
        {
            var result = new SpanResult(TOKEN_TYPE, span);
            string token = span.GetText();
            int position = span.Start + token.Length;

            if (_tokens.Contains(token))
            {
                result.AddSpan<GLSLClassifierTag>(span);
            }

            return result;
        }

        public void Clear() { }
    }
}

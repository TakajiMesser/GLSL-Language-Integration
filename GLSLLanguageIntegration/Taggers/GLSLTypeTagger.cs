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

        public GLSLSpanResult Match(string token, int position, SnapshotSpan span)
        {
            var result = new GLSLSpanResult(TOKEN_TYPE, span);

            if (_tokens.Contains(token))
            {
                var builder = new SpanBuilder()
                {
                    Snapshot = span.Snapshot,
                    Start = position - token.Length,
                    End = position
                };

                result.Consumed = 0;
                result.AddSpan<GLSLClassifierTag>(builder.ToSpan());
            }

            return result;
        }

        public void Clear() { }
    }
}

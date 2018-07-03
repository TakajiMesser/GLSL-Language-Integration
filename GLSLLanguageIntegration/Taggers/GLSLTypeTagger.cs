using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tags;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLTypeTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Type;

        private TokenSet _tokens = new TokenSet(Resources.Types);

        public string GetQuickInfo(string token)
        {
            if (_tokens.Contains(token))
            {
                return _tokens.GetDescription(token);
            }
            else
            {
                return null;
            }
        }

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
                result.AddSpan<GLSLTokenTag>(builder.ToSpan());
            }

            return result;
        }
    }
}

using GLSLLanguageIntegration.Classification;
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

        public SpanResult Match(SnapshotSpan span)
        {
            var result = new SpanResult(TOKEN_TYPE, span);

            string token = span.GetText();
            int position = span.Start + token.Length;

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

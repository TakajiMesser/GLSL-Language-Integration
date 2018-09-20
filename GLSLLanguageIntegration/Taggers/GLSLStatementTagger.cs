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

            /*if (_tokens.Contains(token))
            {
                var builder = new SpanBuilder()
                {
                    Snapshot = span.Snapshot,
                    Start = position - token.Length,
                    End = position
                };

                result.Consumed = 0;
                result.AddSpan<GLSLTokenTag>(builder.ToSpan());
            }*/

            return result;
        }

        public void Clear() { }
    }
}

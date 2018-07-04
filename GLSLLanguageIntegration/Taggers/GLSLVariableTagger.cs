using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tags;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLVariableTagger : IGLSLTagger
    {
        public const GLSLTokenTypes INPUT_TOKEN_TYPE = GLSLTokenTypes.InputVariable;
        public const GLSLTokenTypes OUTPUT_TOKEN_TYPE = GLSLTokenTypes.OutputVariable;
        public const GLSLTokenTypes UNIFORM_TOKEN_TYPE = GLSLTokenTypes.UniformVariable;
        public const GLSLTokenTypes BUFFER_TOKEN_TYPE = GLSLTokenTypes.BufferVariable;
        public const GLSLTokenTypes SHARED_TOKEN_TYPE = GLSLTokenTypes.SharedVariable;
        public const GLSLTokenTypes BUILT_IN_TOKEN_TYPE = GLSLTokenTypes.SharedVariable;

        private TokenSet _tokens = new TokenSet(Resources.Identifiers);

        public object GetQuickInfo(string token) => _tokens.Contains(token) ? _tokens.GetInfo(token).ToQuickInfo() : null;

        public GLSLSpanResult Match(string token, int position, SnapshotSpan span)
        {
            var result = new GLSLSpanResult(BUILT_IN_TOKEN_TYPE, span);

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

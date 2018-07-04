using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tags;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLConstantTagger : IGLSLTagger
    {
        public const GLSLTokenTypes INT_TOKEN_TYPE = GLSLTokenTypes.IntegerConstant;
        public const GLSLTokenTypes FLOAT_TOKEN_TYPE = GLSLTokenTypes.FloatingConstant;
        public const GLSLTokenTypes BUILT_IN_TOKEN_TYPE = GLSLTokenTypes.BuiltInConstant;

        public GLSLSpanResult Match(string token, int position, SnapshotSpan span)
        {
            var result = new GLSLSpanResult(INT_TOKEN_TYPE, span);

            /*var builder = new SpanBuilder()
            {
                Snapshot = span.Snapshot,
                Start = position - token.Length,
                End = position
            };

            result.Consumed = 0;
            result.AddSpan<GLSLTokenTag>(builder.ToSpan());*/

            return result;
        }
    }
}

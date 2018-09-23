using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using System.Text.RegularExpressions;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLConstantTagger : IGLSLTagger
    {
        public const GLSLTokenTypes INT_TOKEN_TYPE = GLSLTokenTypes.IntegerConstant;
        public const GLSLTokenTypes FLOAT_TOKEN_TYPE = GLSLTokenTypes.FloatingConstant;
        public const GLSLTokenTypes BUILT_IN_TOKEN_TYPE = GLSLTokenTypes.BuiltInConstant;

        private TokenSet _builtInTokens = new TokenSet(Resources.Constants, BUILT_IN_TOKEN_TYPE);

        public object GetQuickInfo(string token) => _builtInTokens.Contains(token) ? _builtInTokens.GetInfo(token).ToQuickInfo() : null;

        public SpanResult Match(SnapshotSpan span)
        {
            var result = new SpanResult();

            string token = span.GetText();
            int position = span.Start + token.Length;

            if (_builtInTokens.Contains(token))
            {
                result = new SpanResult(BUILT_IN_TOKEN_TYPE, span);
                result.AddSpan<GLSLClassifierTag>(span);
            }
            else if (IsFloatingConstant(token))
            {
                result = new SpanResult(FLOAT_TOKEN_TYPE, span);
                result.AddSpan<GLSLClassifierTag>(span);
            }
            else if (IsIntegerConstant(token))
            {
                result = new SpanResult(INT_TOKEN_TYPE, span);
                result.AddSpan<GLSLClassifierTag>(span);
            }

            return result;
        }

        // By this point we have already split the token on the '.' character, so this will never match a decimal value...
        private bool IsFloatingConstant(string token) => Regex.Match(token, @"^\d+\.\d+(f|F|lf|LF)?$").Success;

        private bool IsIntegerConstant(string token) => Regex.Match(token, @"^(0x|0X)*\d+(u|U)*$").Success;

        public void Clear() { }
    }
}

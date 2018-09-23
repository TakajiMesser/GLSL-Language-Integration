using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
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

        public TokenTagCollection Match(SnapshotSpan span)
        {
            var tokenTags = new TokenTagCollection(span);

            string token = span.GetText();
            int position = span.Start + token.Length;

            if (_builtInTokens.Contains(token))
            {
                tokenTags.SetClassifierTag(BUILT_IN_TOKEN_TYPE);
            }
            else if (IsFloatingConstant(token))
            {
                tokenTags.SetClassifierTag(FLOAT_TOKEN_TYPE);
            }
            else if (IsIntegerConstant(token))
            {
                tokenTags.SetClassifierTag(INT_TOKEN_TYPE);
            }

            return tokenTags;
        }

        // By this point we have already split the token on the '.' character, so this will never match a decimal value...
        private bool IsFloatingConstant(string token) => Regex.Match(token, @"^\d+\.\d+(f|F|lf|LF)?$").Success;

        private bool IsIntegerConstant(string token) => Regex.Match(token, @"^(0x|0X)*\d+(u|U)*$").Success;

        public void Clear() { }
    }
}

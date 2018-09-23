using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLFunctionTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Function;
        public const GLSLTokenTypes BUILT_IN_TOKEN_TYPE = GLSLTokenTypes.BuiltInFunction;

        private List<TagSpan<IGLSLTag>> _localFunctions = new List<TagSpan<IGLSLTag>>();

        //private Dictionary<string, VariableInfo> _variableInfoByToken = new Dictionary<string, VariableInfo>();
        private List<FunctionInfo> _functionInfos = new List<FunctionInfo>();

        private FunctionSet _tokens = new FunctionSet(Resources.Functions, BUILT_IN_TOKEN_TYPE);

        public object GetQuickInfo(string token)
        {
            if (_tokens.Contains(token))
            {
                return _tokens.GetInfo(token).ToQuickInfo();
            }
            else
            {
                var functionInfo = _functionInfos.FirstOrDefault(v => v.Token == token);
                if (functionInfo != null)
                {
                    return functionInfo.ToQuickInfo();
                }
            }

            return null;
        }

        public TokenTagCollection AddToken(SnapshotSpan span, string returnType)
        {
            var token = span.GetText();
            var position = span.End;

            var tokenTags = new TokenTagCollection(span);
            tokenTags.SetClassifierTag(GLSLTokenTypes.Function);

            _localFunctions.Add(new TagSpan<IGLSLTag>(span, new GLSLClassifierTag(GLSLTokenTypes.Function)));

            var functionInfo = new FunctionInfo(token, GLSLTokenTypes.Function);
            functionInfo.Overloads.Add(new FunctionOverload(returnType));

            _functionInfos.Add(functionInfo);

            return tokenTags;
        }

        public TokenTagCollection Match(SnapshotSpan span)
        {
            var tokenTags = new TokenTagCollection(span);

            string token = span.GetText();
            var matchType = MatchTokenType(token);

            if (matchType != GLSLTokenTypes.None)
            {
                tokenTags.SetClassifierTag(matchType);
            }

            return tokenTags;
        }

        public GLSLTokenTypes MatchTokenType(string token)
        {
            if (_tokens.Contains(token))
            {
                return GLSLTokenTypes.BuiltInFunction;
            }
            else if (_localFunctions.Any(v => v.Span.GetText() == token))
            {
                return GLSLTokenTypes.Function;
            }
            else
            {
                return GLSLTokenTypes.None;
            }
        }

        public void Clear() { }
    }
}

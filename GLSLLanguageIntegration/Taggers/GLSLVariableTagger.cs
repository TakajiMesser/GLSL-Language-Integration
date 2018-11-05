using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLVariableTagger// : IGLSLTagger
    {
        public const GLSLTokenTypes INPUT_TOKEN_TYPE = GLSLTokenTypes.InputVariable;
        public const GLSLTokenTypes OUTPUT_TOKEN_TYPE = GLSLTokenTypes.OutputVariable;
        public const GLSLTokenTypes UNIFORM_TOKEN_TYPE = GLSLTokenTypes.UniformVariable;
        public const GLSLTokenTypes BUFFER_TOKEN_TYPE = GLSLTokenTypes.BufferVariable;
        public const GLSLTokenTypes SHARED_TOKEN_TYPE = GLSLTokenTypes.SharedVariable;
        public const GLSLTokenTypes BUILT_IN_TOKEN_TYPE = GLSLTokenTypes.BuiltInVariable;
        public const GLSLTokenTypes LOCAL_TOKEN_TYPE = GLSLTokenTypes.LocalVariable;

        private VariableInfoCollection _variableInfos = new VariableInfoCollection();
        private TokenSet _tokens = new TokenSet(Resources.Identifiers, BUILT_IN_TOKEN_TYPE);

        public object GetQuickInfo(string token, Scope scope)
        {
            if (_tokens.Contains(token))
            {
                return _tokens.GetInfo(token).ToQuickInfo();
            }
            else
            {
                var variableInfo = _variableInfos.GetVariable(token, scope);
                if (variableInfo != null)
                {
                    return variableInfo.ToQuickInfo();
                }
            }

            return null;
        }

        public IEnumerable<VariableInfo> GetVariables(Scope scope) => _variableInfos.GetVariables(scope);

        public TokenTagCollection AddToken(SnapshotSpan span, Scope scope, string variableType, GLSLTokenTypes tokenType)
        {
            var tokenTags = new TokenTagCollection(span)
            {
                ClassifierTagSpan = new TagSpan<GLSLClassifierTag>(span, new GLSLClassifierTag(tokenType))
            };

            // TODO - We need to check the span to ensure that this is a valid variable name
            // If not, we need to mark it as an error

            _variableInfos.Add(new VariableInfo(span, scope, variableType, tokenType));

            return tokenTags;
        }

        public TokenTagCollection Match(SnapshotSpan span, Scope scope)
        {
            var tokenTags = new TokenTagCollection(span);

            string token = span.GetText();
            var matchType = MatchTokenType(token, scope);

            if (matchType != GLSLTokenTypes.None)
            {
                tokenTags.SetClassifierTag(matchType);
            }

            return tokenTags;
        }

        public GLSLTokenTypes MatchTokenType(string token, Scope scope)
        {
            if (_tokens.Contains(token))
            {
                return GLSLTokenTypes.BuiltInVariable;
            }
            else
            {
                var variableInfo = _variableInfos.GetVariable(token, scope);
                if (variableInfo != null)
                {
                    return variableInfo.GLSLType;
                }
            }

            return GLSLTokenTypes.None;
        }

        public void Translate(ITextSnapshot textSnapshot) => _variableInfos.Translate(textSnapshot);

        public void Remove(Span span) => _variableInfos.Remove(span);

        public void Clear()
        {
            _variableInfos.Clear();
        }
    }
}

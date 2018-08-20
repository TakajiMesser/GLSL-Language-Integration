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
    public class GLSLVariableTagger : IGLSLTagger
    {
        public const GLSLTokenTypes INPUT_TOKEN_TYPE = GLSLTokenTypes.InputVariable;
        public const GLSLTokenTypes OUTPUT_TOKEN_TYPE = GLSLTokenTypes.OutputVariable;
        public const GLSLTokenTypes UNIFORM_TOKEN_TYPE = GLSLTokenTypes.UniformVariable;
        public const GLSLTokenTypes BUFFER_TOKEN_TYPE = GLSLTokenTypes.BufferVariable;
        public const GLSLTokenTypes SHARED_TOKEN_TYPE = GLSLTokenTypes.SharedVariable;
        public const GLSLTokenTypes BUILT_IN_TOKEN_TYPE = GLSLTokenTypes.BuiltInVariable;
        public const GLSLTokenTypes LOCAL_TOKEN_TYPE = GLSLTokenTypes.LocalVariable;

        private List<TagSpan<IGLSLTag>> _inputVariables = new List<TagSpan<IGLSLTag>>();
        private List<TagSpan<IGLSLTag>> _outputVariables = new List<TagSpan<IGLSLTag>>();
        private List<TagSpan<IGLSLTag>> _uniformVariables = new List<TagSpan<IGLSLTag>>();
        private List<TagSpan<IGLSLTag>> _localVariables = new List<TagSpan<IGLSLTag>>();

        //private Dictionary<string, VariableInfo> _variableInfoByToken = new Dictionary<string, VariableInfo>();
        //private List<VariableInfo> _variableInfos = new List<VariableInfo>();
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

        public GLSLSpanResult AddToken(string token, Scope scope, string variableType, int position, SnapshotSpan span, GLSLTokenTypes type)
        {
            var builder = new SpanBuilder()
            {
                Snapshot = span.Snapshot,
                Start = position - token.Length,
                End = position
            };

            var result = new GLSLSpanResult(type, span);
            result.AddSpan<GLSLClassifierTag>(builder.ToSpan());
            
            switch (type)
            {
                case GLSLTokenTypes.InputVariable:
                    _inputVariables.Add(new TagSpan<IGLSLTag>(builder.ToSpan(), new GLSLClassifierTag(type)));
                    break;
                case GLSLTokenTypes.OutputVariable:
                    _outputVariables.Add(new TagSpan<IGLSLTag>(builder.ToSpan(), new GLSLClassifierTag(type)));
                    break;
                case GLSLTokenTypes.UniformVariable:
                    _uniformVariables.Add(new TagSpan<IGLSLTag>(builder.ToSpan(), new GLSLClassifierTag(type)));
                    break;
                case GLSLTokenTypes.LocalVariable:
                    _localVariables.Add(new TagSpan<IGLSLTag>(builder.ToSpan(), new GLSLClassifierTag(type)));
                    break;
            }

            _variableInfos.Add(new VariableInfo(token, scope, variableType, type));
            //_variableInfoByToken[token] = new VariableInfo(token, scope, variableType, type);

            return result;
        }

        public GLSLSpanResult Match(SnapshotSpan span)
        {
            string token = span.GetText();
            int position = span.Start + token.Length;

            var matchType = MatchTokenType(token);

            if (matchType.HasValue)
            {
                var builder = new SpanBuilder()
                {
                    Snapshot = span.Snapshot,
                    Start = position - token.Length,
                    End = position
                };

                var result = new GLSLSpanResult(matchType.Value, span);
                result.AddSpan<GLSLClassifierTag>(builder.ToSpan());

                return result;
            }
            else
            {
                return new GLSLSpanResult();
            }
        }

        public GLSLTokenTypes? MatchTokenType(string token)
        {
            if (_tokens.Contains(token))
            {
                return GLSLTokenTypes.BuiltInVariable;
            }
            else if (_inputVariables.Any(v => v.Span.GetText() == token))
            {
                return GLSLTokenTypes.InputVariable;
            }
            else if (_outputVariables.Any(v => v.Span.GetText() == token))
            {
                return GLSLTokenTypes.OutputVariable;
            }
            else if (_uniformVariables.Any(v => v.Span.GetText() == token))
            {
                return GLSLTokenTypes.UniformVariable;
            }
            else if (_localVariables.Any(v => v.Span.GetText() == token))
            {
                return GLSLTokenTypes.LocalVariable;
            }
            else
            {
                return null;
            }
        }

        public void Clear()
        {
            _inputVariables.Clear();
            _outputVariables.Clear();
            _uniformVariables.Clear();
            _localVariables.Clear();

            _variableInfos.Clear();
        }
    }
}

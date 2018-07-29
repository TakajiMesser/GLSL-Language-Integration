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

        private TokenSet _tokens = new TokenSet(Resources.Functions, BUILT_IN_TOKEN_TYPE);

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

        public GLSLSpanResult AddToken(string token, string returnType, int position, SnapshotSpan span)
        {
            var builder = new SpanBuilder()
            {
                Snapshot = span.Snapshot,
                Start = position - token.Length,
                End = position
            };

            var result = new GLSLSpanResult(GLSLTokenTypes.Function, span);
            result.AddSpan<GLSLClassifierTag>(builder.ToSpan());

            _localFunctions.Add(new TagSpan<IGLSLTag>(builder.ToSpan(), new GLSLClassifierTag(GLSLTokenTypes.Function)));
            _functionInfos.Add(new FunctionInfo(token, returnType, GLSLTokenTypes.Function));

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
                return GLSLTokenTypes.BuiltInFunction;
            }
            else if (_localFunctions.Any(v => v.Span.GetText() == token))
            {
                return GLSLTokenTypes.Function;
            }
            else
            {
                return null;
            }
        }

        public void Clear() { }
    }
}

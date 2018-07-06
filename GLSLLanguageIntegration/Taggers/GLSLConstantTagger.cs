﻿using GLSLLanguageIntegration.Classification;
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

        private TokenSet _builtInTokens = new TokenSet(Resources.Constants);

        public object GetQuickInfo(string token) => _builtInTokens.Contains(token) ? _builtInTokens.GetInfo(token).ToQuickInfo() : null;

        public GLSLSpanResult Match(string token, int position, SnapshotSpan span)
        {
            var result = new GLSLSpanResult();

            var builder = new SpanBuilder()
            {
                Snapshot = span.Snapshot,
                Start = position - token.Length,
                End = position
            };

            if (_builtInTokens.Contains(token))
            {
                result = new GLSLSpanResult(BUILT_IN_TOKEN_TYPE, span);
                result.AddSpan<GLSLClassifierTag>(builder.ToSpan());
            }
            else if (IsIntegerConstant(token))
            {
                result = new GLSLSpanResult(INT_TOKEN_TYPE, span);
                result.AddSpan<GLSLClassifierTag>(builder.ToSpan());
            }
            else if (IsFloatingConstant(token))
            {
                result = new GLSLSpanResult(FLOAT_TOKEN_TYPE, span);
                result.AddSpan<GLSLClassifierTag>(builder.ToSpan());
            }

            return result;
        }

        private bool IsIntegerConstant(string token)
        {
            return Regex.Match(token, @"(0x|0X)*\d+(u|U)*").Success;
        }

        private bool IsFloatingConstant(string token)
        {
            return Regex.Match(token, @"\d+\.\d+(f|F|lf|LF)?").Success;
        }

        public void Clear() { }
    }
}

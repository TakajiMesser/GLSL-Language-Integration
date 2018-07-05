﻿using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tags;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLStatementTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Semicolon;

        public GLSLSpanResult Match(string token, int position, SnapshotSpan span)
        {
            var result = new GLSLSpanResult(TOKEN_TYPE, span);

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
    }
}
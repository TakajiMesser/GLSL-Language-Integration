using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLSLLanguageIntegration.Properties;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GLSLLanguageIntegration.Tags.Spans
{
    public static class GLSLTypeSpan
    {
        public const GLSLTokenTypes TOKEN_TAG = GLSLTokenTypes.Type;

        private static TokenSet _tokens = new TokenSet(Resources.Types);

        public static bool IsType(string token) => _tokens.Contains(token);

        public static GLSLSpanMatch Match(string token, int position, SnapshotSpan span)
        {
            if (_tokens.Contains(token))
            {
                var builder = new SpanBuilder()
                {
                    Snapshot = span.Snapshot,
                    Start = position - token.Length,
                    End = position
                };

                return GLSLSpanMatch.Matched(span, token, builder.ToSpan(), TOKEN_TAG);
            }
            else
            {
                return GLSLSpanMatch.Unmatched();
            }
        }
    }
}

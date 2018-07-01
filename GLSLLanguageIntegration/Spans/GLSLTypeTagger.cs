using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Tags;
using GLSLLanguageIntegration.Tags.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GLSLLanguageIntegration.Spans
{
    public class GLSLTypeTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Type;

        private TokenSet _tokens = new TokenSet(Resources.Types);

        public GLSLSpanResult Match(string token, int position, SnapshotSpan span)
        {
            var result = new GLSLSpanResult(TOKEN_TYPE, span);

            if (_tokens.Contains(token))
            {
                var builder = new SpanBuilder()
                {
                    Snapshot = span.Snapshot,
                    Start = position - token.Length,
                    End = position
                };

                result.Consumed = token.Length;
                result.AddSpan(builder.ToSpan());
            }

            return result;
        }
    }
}

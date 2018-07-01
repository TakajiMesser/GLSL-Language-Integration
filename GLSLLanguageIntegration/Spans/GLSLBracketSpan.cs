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
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GLSLLanguageIntegration.Spans
{
    public static class GLSLBracketSpan
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Bracket;

        private static List<SnapshotSpan> _bracketSpans = new List<SnapshotSpan>();
        private static SpanBuilder _bracketBuilder = new SpanBuilder();

        public static GLSLSpanResult Match(char token, int position, SnapshotSpan span)
        {
            _bracketBuilder.Snapshot = span.Snapshot;

            switch (token)
            {
                case '{':
                    //return GLSLSpanResult.Matched(span, token.ToString(), new SnapshotSpan(span.Snapshot, position, 1), TOKEN_TAG);
                    break;
                case '}':
                    //return GLSLSpanResult.Matched(span, token.ToString(), new SnapshotSpan(span.Snapshot, position, 1), TOKEN_TAG);
                    break;
            }

            return new GLSLSpanResult(TOKEN_TYPE, span);
        }
    }
}

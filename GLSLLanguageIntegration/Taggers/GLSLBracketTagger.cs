using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tags;
using Microsoft.VisualStudio.Text;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Taggers
{
    public static class GLSLBracketTagger
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

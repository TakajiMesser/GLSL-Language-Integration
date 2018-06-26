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
    public static class GLSLPreprocessorSpan
    {
        public const GLSLTokenTypes TOKEN_TAG = GLSLTokenTypes.Preprocessor;

        /*private static SpanSet _spans = new SpanSet(Resources.Directives);

        public static bool IsPreprocessor(string span)
        {
            if (_spans.Contains(span))
            {
                return true;
            }

            return false;
        }*/

        public static GLSLSpanMatch Match(SnapshotSpan span, string token, IEnumerable<string> remainingTokens, int spanIndex)
        {
            if (token.StartsWith("#"))
            {
                int spanStart = span.Start.Position + spanIndex;
                int spanLength = span.Length - spanIndex;

                return GLSLSpanMatch.Matched(TOKEN_TAG, spanStart, spanLength, remainingTokens.Count(), span);
            }
            else
            {
                return GLSLSpanMatch.Unmatched();
            }
        }
    }
}

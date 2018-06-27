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
    public static class GLSLBracketSpan
    {
        public const GLSLTokenTypes TOKEN_TAG = GLSLTokenTypes.Bracket;

        public static GLSLSpanMatch Match(SnapshotSpan span, IEnumerable<SnapshotSpan> remainingSpans, string token, IEnumerable<string> remainingTokens, int position)
        {
            if (token.Contains("//"))
            {
                int tokenIndex = token.IndexOf("//");

                int start = position + tokenIndex;
                int length = span.End.Position - start;

                return GLSLSpanMatch.Matched(span, TOKEN_TAG, start, length, 1, remainingTokens.Count() + 1);
            }
            else
            {
                return GLSLSpanMatch.Unmatched();
            }
        }
    }
}

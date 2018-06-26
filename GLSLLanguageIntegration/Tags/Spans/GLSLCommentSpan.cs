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
    public static class GLSLCommentSpan
    {
        public const GLSLTokenTypes TOKEN_TAG = GLSLTokenTypes.Comment;

        public static GLSLSpanMatch Match(SnapshotSpan span, string token, IEnumerable<string> remainingTokens, int spanIndex)
        {
            if (token.Contains("//"))
            {
                int tokenIndex = token.IndexOf("//");

                int spanStart = spanIndex + tokenIndex;
                int spanLength = span.Length - spanStart;

                return GLSLSpanMatch.Matched(TOKEN_TAG, span.Start.Position + spanStart, spanLength, remainingTokens.Count(), span);
            }
            else
            {
                return GLSLSpanMatch.Unmatched();
            }
        }
    }
}

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
    public static class GLSLKeywordSpan
    {
        public const GLSLTokenTypes TOKEN_TAG = GLSLTokenTypes.Keyword;

        private static TokenSet _tokens = new TokenSet(Resources.Keywords);

        public static GLSLSpanMatch Match(SnapshotSpan span, string token, IEnumerable<string> remainingTokens, int spanIndex)
        {
            if (_tokens.Contains(token))
            {
                //int tokenIndex = token.IndexOf("//");

                int spanStart = spanIndex;// spanIndex + tokenIndex;
                int spanLength = token.Length;// span.Length - spanStart;

                return GLSLSpanMatch.Matched(TOKEN_TAG, span.Start.Position + spanStart, spanLength, 0, span);
            }
            else
            {
                return GLSLSpanMatch.Unmatched();
            }
        }
    }
}

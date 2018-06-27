﻿using System;
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

        public static GLSLSpanMatch Match(SnapshotSpan span, string token, IEnumerable<string> remainingTokens, int position)
        {
            if (token.StartsWith("#"))
            {
                int start = position;
                int length = span.End.Position - start;

                return GLSLSpanMatch.Matched(span, TOKEN_TAG, start, length, 1, 0);
            }
            else
            {
                return GLSLSpanMatch.Unmatched();
            }
        }
    }
}

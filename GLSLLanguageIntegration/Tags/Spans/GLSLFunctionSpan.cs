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
    public static class GLSLFunctionSpan
    {
        public const GLSLTokenTypes TOKEN_TAG = GLSLTokenTypes.Keyword;

        /*private static SpanSet _spans = new SpanSet(Resources.Functions);

        public static bool IsFunction(string span)
        {
            if (_spans.Contains(span))
            {
                return true;
            }

            return false;
        }*/
    }
}

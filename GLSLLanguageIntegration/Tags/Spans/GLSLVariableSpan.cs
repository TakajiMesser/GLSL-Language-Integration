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
    public static class GLSLVariableSpan
    {
        public const GLSLTokenTypes TOKEN_TAG = GLSLTokenTypes.Identifier;

        /*private static SpanSet _spans = new SpanSet(Resources.Variables);

        /*public static bool IsVariable(string span)
        {
            if (_spans.Contains(span))
            {
                return true;
            }

            return false;
        }*/
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GLSLLanguageIntegration
{
    public class GLSLTokenTag : ITag
    {
        public GLSLTokenTypes TokenType { get; private set; }

        public GLSLTokenTag(GLSLTokenTypes type)
        {
            TokenType = type;
        }
    }
}

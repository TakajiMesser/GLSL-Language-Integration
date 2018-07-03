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

namespace GLSLLanguageIntegration.Tags
{
    public class GLSLOutlineTag : IGLSLTag
    {
        public const string COLLAPSE_TEXT = "...";
        public const string COLLAPSE_HINT = "Hover Text";

        public GLSLTokenTypes TokenType { get; private set; }
        public OutliningRegionTag RegionTag { get; private set; }

        public GLSLOutlineTag(GLSLTokenTypes type, string collapseText = COLLAPSE_TEXT)
        {
            TokenType = type;
            RegionTag = new OutliningRegionTag(false, false, collapseText, COLLAPSE_HINT);
        }
    }
}

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
    public class GLSLOutlineTag : IGLSLTag, IOutliningRegionTag
    {
        public const string COLLAPSE_TEXT = "...";
        public const string COLLAPSE_HINT = "Hover Text";

        public GLSLTokenTypes TokenType { get; private set; }
        public OutliningRegionTag RegionTag { get; private set; }

        public bool IsDefaultCollapsed => throw new NotImplementedException();

        public bool IsImplementation => throw new NotImplementedException();

        public object CollapsedForm => throw new NotImplementedException();

        public object CollapsedHintForm => throw new NotImplementedException();

        public GLSLOutlineTag(GLSLTokenTypes type)
        {
            TokenType = type;
            RegionTag = new OutliningRegionTag(false, false, COLLAPSE_TEXT, COLLAPSE_HINT);
        }
    }
}

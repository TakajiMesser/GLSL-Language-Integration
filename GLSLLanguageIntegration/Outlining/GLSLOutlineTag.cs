using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text.Tagging;

namespace GLSLLanguageIntegration.Outlining
{
    public class GLSLOutlineTag : IGLSLTag
    {
        public const string COLLAPSE_TEXT = "...";
        public const string COLLAPSE_HINT = "Hover Text";

        public GLSLTokenTypes TokenType { get; private set; }
        public OutliningRegionTag RegionTag { get; private set; }

        public GLSLOutlineTag(GLSLTokenTypes type, string collapseText = COLLAPSE_TEXT, string collapseHint = COLLAPSE_HINT)
        {
            TokenType = type;
            RegionTag = new OutliningRegionTag(false, false, collapseText, collapseHint);
        }
    }
}

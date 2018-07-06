using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text.Tagging;

namespace GLSLLanguageIntegration.Outlining
{
    public class GLSLHighlightTag : TextMarkerTag, IGLSLTag
    {
        public GLSLTokenTypes TokenType { get; private set; }

        public GLSLHighlightTag() : base("MarkerFormatDefinition/HighlightWordFormatDefinition") { }
        public GLSLHighlightTag(GLSLTokenTypes type) : base("MarkerFormatDefinition/HighlightWordFormatDefinition")
        {
            TokenType = type;
        }
    }
}

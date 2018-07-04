using Microsoft.VisualStudio.Text.Tagging;

namespace GLSLLanguageIntegration.Tags
{
    public interface IGLSLTag : ITag
    {
        GLSLTokenTypes TokenType { get; }
    }
}

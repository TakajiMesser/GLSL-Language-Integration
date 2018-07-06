using Microsoft.VisualStudio.Text.Tagging;

namespace GLSLLanguageIntegration.Tokens
{
    public interface IGLSLTag : ITag
    {
        GLSLTokenTypes TokenType { get; }
    }
}

using GLSLLanguageIntegration.Tokens;

namespace GLSLLanguageIntegration.Classification
{
    public class GLSLClassifierTag : IGLSLTag
    {
        public GLSLTokenTypes TokenType { get; private set; }

        public GLSLClassifierTag(GLSLTokenTypes type) => TokenType = type;
    }
}

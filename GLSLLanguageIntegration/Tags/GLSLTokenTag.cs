namespace GLSLLanguageIntegration.Tags
{
    public class GLSLTokenTag : IGLSLTag
    {
        public GLSLTokenTypes TokenType { get; private set; }

        public GLSLTokenTag(GLSLTokenTypes type) => TokenType = type;
    }
}

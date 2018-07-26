using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Spans
{
    public class TokenResult
    {
        public string Token { get; set; }
        public GLSLTokenTypes? TokenType { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition => StartPosition + Token.Length;

        public SnapshotSpan Span { get; set; }

        public TokenResult(string token, int position)
        {
            Token = token;
            StartPosition = position;
        }
    }
}

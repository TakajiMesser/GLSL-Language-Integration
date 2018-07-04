namespace GLSLLanguageIntegration.Tokens
{
    public class TokenInfo
    {
        public string Token { get; private set; }
        public string Definition { get; set; }

        public TokenInfo(string token)
        {
            Token = token;
        }

        public object ToQuickInfo()
        {
            return Definition;
        }
    }
}

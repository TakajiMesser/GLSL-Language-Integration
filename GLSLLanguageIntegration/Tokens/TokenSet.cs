using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GLSLLanguageIntegration.Tokens
{
    public class TokenSet
    {
        private Dictionary<string, TokenInfo> _infoByToken = new Dictionary<string, TokenInfo>();

        public TokenSet(string contents, GLSLTokenTypes glslType)
        {
            ParseContentForTokens(contents, glslType);
        }

        private void ParseContentForTokens(string contents, GLSLTokenTypes glslType)
        {
            var lines = new List<string>();

            foreach (var line in Regex.Split(contents, Environment.NewLine))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (lines.Count > 0)
                    {
                        var names = lines.First().Split(',');
                        string description = null;

                        if (lines.Count > 1)
                        {
                            description = string.Join(Environment.NewLine, lines.Skip(1));
                        }

                        foreach (var name in names)
                        {
                            var tokenInfo = new TokenInfo(name, glslType)
                            {
                                Definition = description
                            };

                            _infoByToken[name] = tokenInfo;
                        }

                        lines.Clear();
                    }
                }
                else
                {
                    lines.Add(line);
                }
            }
        }

        public bool Contains(string token) => _infoByToken.ContainsKey(token);
        public TokenInfo GetInfo(string token) => _infoByToken[token];
    }
}

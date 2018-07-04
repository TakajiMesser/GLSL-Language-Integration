using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GLSLLanguageIntegration.Tokens
{
    public class TokenSet
    {
        private Dictionary<string, TokenInfo> _infoByToken = new Dictionary<string, TokenInfo>();

        public TokenSet(string contents)
        {
            var lines = new List<string>();

            foreach (var line in Regex.Split(contents, Environment.NewLine))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (lines.Count > 0)
                    {
                        string description = "";
                        if (lines.Count > 1)
                        {
                            description = lines.Last();
                            lines.RemoveAt(lines.Count - 1);
                        }

                        foreach (var token in lines)
                        {
                            var tokenInfo = new TokenInfo(token)
                            {
                                Definition = description
                            };

                            _infoByToken.Add(token, tokenInfo);
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

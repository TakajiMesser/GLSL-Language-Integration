using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GLSLLanguageIntegration.Tokens
{
    public class TokenSet
    {
        private Dictionary<string, string> _descriptionByToken = new Dictionary<string, string>();

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
                            _descriptionByToken.Add(token, description);
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

        public bool Contains(string token) => _descriptionByToken.ContainsKey(token);
        public string GetDescription(string token) => _descriptionByToken[token];
    }
}

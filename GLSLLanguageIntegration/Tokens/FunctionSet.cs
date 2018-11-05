using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GLSLLanguageIntegration.Tokens
{
    public class FunctionSet
    {
        private Dictionary<string, FunctionInfo> _infoByToken = new Dictionary<string, FunctionInfo>();

        public FunctionSet(string contents, GLSLTokenTypes glslType)
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
                        var match = Regex.Match(lines.First(),
                            @"^(?<returnType>[a-zA-Z0-9]+) (?<name>[a-zA-Z0-9]+) \(((?<parameterType>[a-zA-Z0-9]+) (?<parameterName>[a-zA-Z_0-9]+)(, )?)*\)( )*$");

                        if (match.Success)
                        {
                            var returnType = match.Groups["returnType"].Value;
                            var name = match.Groups["name"].Value;

                            string definition = null;

                            if (lines.Count > 1)
                            {
                                definition = string.Join(Environment.NewLine, lines.Skip(1));
                            }

                            if (!_infoByToken.ContainsKey(name))
                            {
                                _infoByToken[name] = new FunctionInfo(name, glslType)
                                {
                                    Definition = definition
                                };
                            }

                            var functionOverload = new FunctionOverload(returnType)
                            {
                                Documentation = definition
                            };

                            if (match.Groups["parameterType"].Success && match.Groups["parameterName"].Success)
                            {
                                for (var i = 0; i < match.Groups["parameterType"].Captures.Count && i < match.Groups["parameterName"].Captures.Count; i++)
                                {
                                    var parameterType = match.Groups["parameterType"].Captures[i].Value;
                                    var parameterName = match.Groups["parameterName"].Captures[i].Value;

                                    functionOverload.Parameters.Add(new ParameterInfo(parameterName, parameterType));
                                }
                            }

                            _infoByToken[name].Overloads.Add(functionOverload);
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
        public FunctionInfo GetInfo(string token) => _infoByToken[token];
    }
}

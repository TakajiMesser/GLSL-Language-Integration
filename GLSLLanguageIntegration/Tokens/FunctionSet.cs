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
                        // genType atan (genType y, genType x)
                        // genType atan (genType y_over_x) 
                        // genType sin (genType angle)
                        var match = Regex.Match(lines.First(),
                            //@"^(?<returnType>[a-zA-Z]+) (?<name>[a-zA-Z]+) \([a-zA-Z]\)");
                            @"^(?<returnType>[a-zA-Z]+) (?<name>[a-zA-Z]+) \(((?<parameterType>[a-zA-Z]+) (?<parameterName>[a-zA-Z_]+)(, )?)*\)( )*$");

                        int a = 4;
                        if (lines.First().StartsWith("genType atan"))
                        {
                            a = 3;
                        }

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

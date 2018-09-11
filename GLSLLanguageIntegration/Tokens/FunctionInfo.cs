using GLSLLanguageIntegration.Utilities;
using System;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Tokens
{
    public class FunctionInfo : TokenInfo
    {
        public List<FunctionOverload> Overloads { get; } = new List<FunctionOverload>();

        public FunctionInfo(string token, GLSLTokenTypes glslType) : base(token, glslType) { }

        protected override string GetTitle()
        {
            if (!GLSLType.IsFunction()) throw new NotImplementedException("Could not handle type " + Enum.GetName(typeof(GLSLTokenTypes), GLSLType));
            return "(" + GLSLType.GetDisplayName() + ") " + Token;
        }
    }
}

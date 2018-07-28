using GLSLLanguageIntegration.Utilities;
using System;

namespace GLSLLanguageIntegration.Tokens
{
    public class FunctionInfo : TokenInfo
    {
        public string ReturnType { get; private set; }

        public FunctionInfo(string token, string returnType, GLSLTokenTypes glslType) : base(token, glslType)
        {
            ReturnType = returnType;
        }

        protected override string GetTitle()
        {
            if (!GLSLType.IsFunction()) throw new NotImplementedException("Could not handle type " + Enum.GetName(typeof(GLSLTokenTypes), GLSLType));
            return "(" + GLSLType.GetDisplayName() + ") " + ReturnType + " " + Token;
        }
    }
}

using GLSLLanguageIntegration.Utilities;
using System;

namespace GLSLLanguageIntegration.Tokens
{
    public class VariableInfo : TokenInfo
    {
        public int Scope { get; private set; }
        public string VariableType { get; private set; }

        public VariableInfo(string token, int scope, string variableType, GLSLTokenTypes glslType) : base(token, glslType)
        {
            Scope = scope;
            VariableType = variableType;
        }

        protected override string GetTitle()
        {
            if (!GLSLType.IsVariable()) throw new NotImplementedException("Could not handle type " + Enum.GetName(typeof(GLSLTokenTypes), GLSLType));
            return "(" + GLSLType.GetDisplayName() + ") " + VariableType + " " + Token;
        }
    }
}

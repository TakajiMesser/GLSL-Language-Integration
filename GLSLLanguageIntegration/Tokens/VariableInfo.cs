using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Utilities;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GLSLLanguageIntegration.Tokens
{
    public class VariableInfo : TokenInfo
    {
        public string VariableType { get; private set; }

        public VariableInfo(string token, string variableType, GLSLTokenTypes glslType) : base(token, glslType)
        {
            VariableType = variableType;
        }

        protected override string GetTitle()
        {
            if (!GLSLType.IsVariable()) throw new NotImplementedException("Could not handle type " + Enum.GetName(typeof(GLSLTokenTypes), GLSLType));

            return "(" + GLSLType.GetDisplayName() + ") " + VariableType + " " + Token;
        }
    }
}

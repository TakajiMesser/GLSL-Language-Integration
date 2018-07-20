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
            switch (GLSLType)
            {
                case GLSLTokenTypes.InputVariable:
                    return "(Input Variable) " + VariableType + " " + Token;
                case GLSLTokenTypes.OutputVariable:
                    return "(Output Variable) " + VariableType + " " + Token;
                case GLSLTokenTypes.UniformVariable:
                    return "(Uniform Variable) " + VariableType + " " + Token;
                case GLSLTokenTypes.LocalVariable:
                    return "(Local Variable) " + VariableType + " " + Token;
                default:
                    throw new NotImplementedException("Could not handle type " + Enum.GetName(typeof(GLSLTokenTypes), GLSLType));
            }
        }
    }
}

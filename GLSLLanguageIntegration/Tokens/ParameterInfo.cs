using GLSLLanguageIntegration.Utilities;
using System;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Tokens
{
    public class ParameterInfo
    {
        public string Name { get; private set; }
        public string ReturnType { get; private set; }
        public string Documentation { get; set; }

        public ParameterInfo(string name, string returnType)
        {
            Name = name;
            ReturnType = returnType;
        }
    }
}

using System.Collections.Generic;

namespace GLSLLanguageIntegration.Tokens
{
    public class FunctionOverload
    {
        public string ReturnType { get; private set; }
        public string Documentation { get; set; }
        public List<ParameterInfo> Parameters { get; } = new List<ParameterInfo>();

        public FunctionOverload(string returnType)
        {
            ReturnType = returnType;
        }
    }
}

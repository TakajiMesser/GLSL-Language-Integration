using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Intellisense.Signatures
{
    internal class GLSLParameter : IParameter
    {
        public ISignature Signature { get; private set; }
        public string Name { get; private set; }
        public string Documentation { get; private set; }
        public Span Locus { get; private set; }
        public Span PrettyPrintedLocus { get; private set; }

        public GLSLParameter(string name, string documentation, Span locus, ISignature signature)
        {
            Name = name;
            Documentation = documentation;
            Locus = locus;
            Signature = signature;
        }
    }
}

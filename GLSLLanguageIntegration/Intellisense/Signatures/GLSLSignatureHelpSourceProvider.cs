using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace GLSLLanguageIntegration.Intellisense.Signatures
{
    [Export(typeof(ISignatureHelpSourceProvider))]
    [ContentType("glsl")]
    [Name("glslSignatureHelp")]
    internal class GLSLSignatureHelpSourceProvider : ISignatureHelpSourceProvider
    {
        public ISignatureHelpSource TryCreateSignatureHelpSource(ITextBuffer buffer) => new GLSLSignatureHelpSource(buffer);
    }
}

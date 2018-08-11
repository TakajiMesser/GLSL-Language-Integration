using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace GLSLLanguageIntegration.Intellisense.Completions
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("glsl")]
    [Name("glslCompletion")]
    internal sealed class GLSLCompletionSourceProvider : ICompletionSourceProvider
    {
        public ICompletionSource TryCreateCompletionSource(ITextBuffer buffer) => new GLSLCompletionSource(buffer);
    }
}

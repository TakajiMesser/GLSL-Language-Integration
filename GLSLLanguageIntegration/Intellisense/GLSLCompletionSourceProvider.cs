using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLLanguageIntegration.Intellisense
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("glsl")]
    [Name("glslCompletion")]
    internal sealed class GLSLCompletionSourceProvider : ICompletionSourceProvider
    {
        public ICompletionSource TryCreateCompletionSource(ITextBuffer buffer) => new GLSLCompletionSource(buffer);
    }
}

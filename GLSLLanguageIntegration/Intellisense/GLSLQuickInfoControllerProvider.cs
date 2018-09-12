using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace GLSLLanguageIntegration.Intellisense
{
    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("GLSL QuickInfo Controller")]
    [ContentType("text")]
    internal class GLSLQuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        internal IAsyncQuickInfoBroker QuickInfoBroker { get; set; }

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new GLSLQuickInfoController(textView, subjectBuffers, this);
        }
    }
}

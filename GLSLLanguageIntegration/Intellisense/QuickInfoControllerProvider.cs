using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLLanguageIntegration.Intellisense
{
    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("Template QuickInfo Controller")]
    [ContentType("text")]
    internal class TemplateQuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        internal IQuickInfoBroker QuickInfoBroker { get; set; }

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new TemplateQuickInfoController(textView, subjectBuffers, this);
        }
    }
}

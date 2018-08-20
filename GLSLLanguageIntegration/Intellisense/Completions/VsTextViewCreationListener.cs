using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace GLSLLanguageIntegration.Intellisense.Completions
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("glsl")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class VsTextViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterService;

        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import]
        internal ICompletionBroker CompletionBroker;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView textView = AdapterService.GetWpfTextView(textViewAdapter);
            if (textView != null)
            {
                var textNavigator = NavigatorService.GetTextStructureNavigator(textView.TextBuffer);

                textView.Properties.GetOrCreateSingletonProperty(() =>
                    new CommandFilter(textViewAdapter, textView, textNavigator, CompletionBroker));
            }
        }
    }
}

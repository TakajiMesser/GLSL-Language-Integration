using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Runtime.InteropServices;

namespace GLSLLanguageIntegration.Intellisense.Signatures
{
    internal sealed class CommandFilter : IOleCommandTarget
    {
        private IWpfTextView _textView;
        private IOleCommandTarget _nextCommand;
        private ITextStructureNavigator _textNavigator;

        private ISignatureHelpBroker _signatureBroker;
        private ISignatureHelpSession _signatureSession;

        public CommandFilter(IVsTextView textViewAdapter, IWpfTextView textView, ITextStructureNavigator textNavigator, ISignatureHelpBroker signatureBroker)
        {
            textViewAdapter.AddCommandFilter(this, out _nextCommand);

            _textView = textView;
            _textNavigator = textNavigator;
            _signatureBroker = signatureBroker;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            int hresult = HandleEditorCommands(ref pguidCmdGroup, nCmdID)
                ? VSConstants.S_OK
                : _nextCommand.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

            if (ErrorHandler.Succeeded(hresult) && pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID)nCmdID)
                {
                    case VSConstants.VSStd2KCmdID.TYPECHAR:
                        char typedChar = GetTypeChar(pvaIn);
                        HandleTypeChar(typedChar);
                        break;
                    case VSConstants.VSStd2KCmdID.BACKSPACE:
                        //FilterCompletion();
                        break;
                }
            }

            return hresult;
        }

        private void HandleTypeChar(char typedChar)
        {
            switch (typedChar)
            {
                case '(':
                    StartSignatureSession();
                    break;
                case ')':
                    if (_signatureSession != null)
                    {
                        _signatureSession.Dismiss();
                        _signatureSession = null;
                    }
                    break;
            }
        }

        private bool HandleEditorCommands(ref Guid pguidCmdGroup, uint nCmdID)
        {
            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID)nCmdID)
                {
                    case VSConstants.VSStd2KCmdID.CANCEL:
                        return Cancel();
                }
            }

            return false;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID)prgCmds[0].cmdID)
                {
                    case VSConstants.VSStd2KCmdID.AUTOCOMPLETE:
                    case VSConstants.VSStd2KCmdID.COMPLETEWORD:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                        return VSConstants.S_OK;
                }
            }

            return _nextCommand.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        private char GetTypeChar(IntPtr pvaIn) => (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);

        /// <summary>
        /// Cancel the auto-complete session, and leave the text unmodified
        /// </summary>
        private bool Cancel()
        {
            if (_signatureSession != null)
            {
                _signatureSession.Dismiss();
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool StartSignatureSession()
        {
            if (_signatureSession == null)
            {
                // Move the caret back to the preceding word
                SnapshotPoint caret = _textView.Caret.Position.BufferPosition - 1;
                TextExtent extent = _textNavigator.GetExtentOfWord(caret);
                string word = extent.Span.GetText();

                _signatureSession = _signatureBroker.TriggerSignatureHelp(_textView);

                _signatureSession.Dismissed += (sender, args) => _signatureSession = null;
                _signatureSession.Start();

                return true;
            }

            return false;
        }
    }
}

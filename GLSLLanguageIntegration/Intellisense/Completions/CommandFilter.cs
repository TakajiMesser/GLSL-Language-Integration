using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace GLSLLanguageIntegration.Intellisense.Completions
{
    internal sealed class CommandFilter : IOleCommandTarget
    {
        private IWpfTextView _textView;
        private IOleCommandTarget _nextCommand;
        private ITextStructureNavigator _textNavigator;

        private ICompletionBroker _completionBroker;
        private ICompletionSession _completionSession;

        public CommandFilter(IVsTextView textViewAdapter, IWpfTextView textView, ITextStructureNavigator textNavigator, ICompletionBroker completionBroker)
        {
            textViewAdapter.AddCommandFilter(this, out _nextCommand);

            _textView = textView;
            _textNavigator = textNavigator;
            _completionBroker = completionBroker;
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

            //await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            ThreadHelper.ThrowIfNotOnUIThread();
            return _nextCommand.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            //await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            ThreadHelper.ThrowIfNotOnUIThread();

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
                        FilterCompletion();
                        break;
                }
            }

            return hresult;
        }

        private void HandleTypeChar(char typedChar)
        {
            switch (typedChar)
            {
                case var value when char.IsLetter(typedChar) && _completionSession == null:
                    StartSession();
                    break;
                case var value when char.IsLetterOrDigit(typedChar) && _completionSession != null:
                    FilterCompletion();
                    break;
            }
        }

        private bool HandleEditorCommands(ref Guid pguidCmdGroup, uint nCmdID)
        {
            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID)nCmdID)
                {
                    case VSConstants.VSStd2KCmdID.AUTOCOMPLETE:
                    case VSConstants.VSStd2KCmdID.COMPLETEWORD:
                        return StartSession();
                    case VSConstants.VSStd2KCmdID.RETURN:
                        return Complete(false);
                    case VSConstants.VSStd2KCmdID.TAB:
                        return Complete(true);
                    case VSConstants.VSStd2KCmdID.CANCEL:
                        return Cancel();
                }
            }

            return false;
        }

        private char GetTypeChar(IntPtr pvaIn) => (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);

        /// <summary>
        /// Narrow down the list of options as the user types input
        /// </summary>
        private void FilterCompletion()
        {
            if (_completionSession != null)
            {
                //_completionSession.Filter();
                //_completionSession.SelectedCompletionSet.Filter();
                _completionSession.SelectedCompletionSet.SelectBestMatch();
                _completionSession.SelectedCompletionSet.Recalculate();
            }
        }

        /// <summary>
        /// Cancel the auto-complete session, and leave the text unmodified
        /// </summary>
        private bool Cancel()
        {
            if (_completionSession != null)
            {
                _completionSession.Dismiss();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Auto-complete text using the specified token
        /// </summary>
        private bool Complete(bool force)
        {
            if (_completionSession != null)
            {
                if (_completionSession.SelectedCompletionSet.SelectionStatus.IsSelected || force)
                {
                    _completionSession.Commit();
                    return true;
                }
                else
                {
                    _completionSession.Dismiss();
                }
            }

            return false;
        }

        /// <summary>
        /// Display list of potential tokens
        /// </summary>
        private bool StartSession()
        {
            if (_completionSession == null)
            {
                SnapshotPoint caret = _textView.Caret.Position.BufferPosition;
                ITextSnapshot snapshot = caret.Snapshot;

                _completionSession = _completionBroker.IsCompletionActive(_textView)
                    ? _completionBroker.GetSessions(_textView).First()
                    : _completionBroker.CreateCompletionSession(_textView, snapshot.CreateTrackingPoint(caret, PointTrackingMode.Positive), true);

                _completionSession.Dismissed += (sender, args) => _completionSession = null;
                _completionSession.Start();

                return true;
            }

            return false;
        }
    }
}

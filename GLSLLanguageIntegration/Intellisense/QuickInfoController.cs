﻿using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Intellisense
{
    internal class TemplateQuickInfoController : IIntellisenseController
    {
        private ITextView _textView;
        private IList<ITextBuffer> _subjectBuffers;
        private TemplateQuickInfoControllerProvider _componentContext;
        private IAsyncQuickInfoSession _session;

        internal TemplateQuickInfoController(ITextView textView, IList<ITextBuffer> subjectBuffers, TemplateQuickInfoControllerProvider componentContext)
        {
            _textView = textView;
            _subjectBuffers = subjectBuffers;
            _componentContext = componentContext;

            _textView.MouseHover += OnTextViewMouseHover;
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer) { }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer) { }

        public void Detach(ITextView textView)
        {
            if (_textView == textView)
            {
                _textView.MouseHover -= OnTextViewMouseHover;
                _textView = null;
            }
        }

        /// <summary>
        /// Determine if the mouse is hovering over a token. If so, highlight the token and display QuickInfo
        /// </summary>
        private async void OnTextViewMouseHover(object sender, MouseHoverEventArgs e)
        {
            SnapshotPoint? point = GetMousePosition(new SnapshotPoint(_textView.TextSnapshot, e.Position));

            if (point.HasValue)
            {
                ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position, PointTrackingMode.Positive);

                // Find the broker for this buffer
                if (!_componentContext.QuickInfoBroker.IsQuickInfoActive(_textView))
                {
                    _session = await _componentContext.QuickInfoBroker.TriggerQuickInfoAsync(_textView, triggerPoint);
                }
            }
        }

        /// <summary>
        /// get mouse location onscreen. Used to determine what word the cursor is currently hovering over
        /// </summary>
        private SnapshotPoint? GetMousePosition(SnapshotPoint topPosition)
        {
            // Map this point down to the appropriate subject buffer.
            return _textView.BufferGraph.MapDownToFirstMatch(topPosition, 
                PointTrackingMode.Positive, 
                snapshot => _subjectBuffers.Contains(snapshot.TextBuffer), 
                PositionAffinity.Predecessor);
        }
    }
}

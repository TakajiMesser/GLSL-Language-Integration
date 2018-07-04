using GLSLLanguageIntegration.Classification;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Intellisense
{
    internal sealed class GLSLCompletionSource : ICompletionSource
    {
        private ITextBuffer _buffer;
        private bool _disposed = false;

        public GLSLCompletionSource(ITextBuffer buffer)
        {
            _buffer = buffer;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (_disposed) throw new ObjectDisposedException("GLSLCompletionSource");

            var completions = new List<Completion>()
            {
                new Completion(nameof(GLSLPreprocessor)),
                new Completion(nameof(GLSLComment)),
                new Completion(nameof(GLSLKeyword))
            };

            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(snapshot);

            if (triggerPoint != null)
            {
                var line = triggerPoint.GetContainingLine();
                var start = triggerPoint;

                while (start > line.Start && !char.IsWhiteSpace((start - 1).GetChar()))
                {
                    start -= 1;
                }

                var applicableTo = snapshot.CreateTrackingSpan(new SnapshotSpan(start, triggerPoint), SpanTrackingMode.EdgeInclusive);

                completionSets.Add(new CompletionSet("All", "All", applicableTo, completions, Enumerable.Empty<Completion>()));
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}

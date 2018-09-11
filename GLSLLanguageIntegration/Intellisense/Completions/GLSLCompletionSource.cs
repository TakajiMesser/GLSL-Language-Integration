using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Intellisense.Completions
{
    internal sealed class GLSLCompletionSource : ICompletionSource
    {
        private ITextBuffer _buffer;
        private ITagAggregator<IGLSLTag> _aggregator;
        private bool _disposed = false;

        public GLSLCompletionSource(ITextBuffer buffer, ITagAggregator<IGLSLTag> aggregator)
        {
            _buffer = buffer;
            _aggregator = aggregator;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (_disposed) throw new ObjectDisposedException("GLSLCompletionSource");

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

                var span = new SnapshotSpan(start, triggerPoint);
                var applicableTo = snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive);

                // Now that we have the span we want, get all tokens from the GLSLTokenTagger, then build our completion set based on those
                // We have our trigger span, but we want to get ALL tokens from the GLSLTokenTagger, because we need to search for our token in those
                /*if (session != null)
                {
                    session.SelectedCompletionSet.SelectBestMatch();
                    session.SelectedCompletionSet.Recalculate();
                }*/

                var tagger = new GLSLTokenTagProvider().CreateTagger<IGLSLTag>(_buffer) as GLSLTokenTagger;
                var completions = tagger.GetCompletions(span);

                completionSets.Add(new CompletionSet("All", "All", applicableTo, completions, Enumerable.Empty<Completion>()));
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}

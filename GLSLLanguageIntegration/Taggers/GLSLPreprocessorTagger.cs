using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLPreprocessorTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.PreprocessorDirective;

        //private static SpanSet _spans = new SpanSet(Resources.Directives);

        private List<SnapshotSpan> _spans = new List<SnapshotSpan>();
        private SpanBuilder _builder = new SpanBuilder();

        public object GetQuickInfo(string token) => new TokenInfo("PreprocessorDirective", TOKEN_TYPE).ToQuickInfo();

        public TokenTagCollection Match(SnapshotSpan span)
        {
            _builder.Snapshot = span.Snapshot;

            var tokenTags = new TokenTagCollection(span);
            string token = span.GetText();
            int position = span.Start + token.Length;

            if (token.StartsWith("#"))
            {
                int start = position - token.Length;

                if (!_builder.Start.HasValue)
                {
                    _builder.Start = start;
                    string text = span.Snapshot.GetText();

                    int nConsumed = _builder.ConsumeUntil(text, start, "\r", "\n");
                    if (nConsumed >= 0)
                    {
                        _builder.End = _builder.Start + nConsumed;
                        var preprocessorSpan = _builder.ToSpan();

                        _builder.Clear();
                        _spans.Add(preprocessorSpan);

                        tokenTags.Consumed = preprocessorSpan.Length - token.Length;
                        tokenTags.ClassifierTagSpan = new TagSpan<GLSLClassifierTag>(preprocessorSpan, new GLSLClassifierTag(TOKEN_TYPE));
                    }
                }
            }

            return tokenTags;
        }

        public void Clear()
        {
            _spans.Clear();
            _builder.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLSLLanguageIntegration.Properties;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GLSLLanguageIntegration.Tags.Spans
{
    public static class GLSLPreprocessorSpan
    {
        public const GLSLTokenTypes TOKEN_TAG = GLSLTokenTypes.Preprocessor;

        //private static SpanSet _spans = new SpanSet(Resources.Directives);

        private static List<SnapshotSpan> _spans = new List<SnapshotSpan>();
        private static SpanBuilder _builder = new SpanBuilder();

        public static GLSLSpanMatch Match(string token, int position, SnapshotSpan span)
        {
            _builder.Snapshot = span.Snapshot;

            if (token.StartsWith("#"))
            {
                int start = position - token.Length;

                if (!_builder.Start.HasValue)
                {
                    _builder.Start = start;
                    string text = span.GetText();

                    for (var i = start - span.Start.Position; i < text.Length; i++)
                    {
                        char character = text[i];

                        // Keep consuming characters until we find a line end
                        if (character == '\r' || character == '\n')
                        {
                            _builder.End = start + i;
                            var preprocessorSpan = _builder.ToSpan();

                            _builder.Clear();
                            _spans.Add(preprocessorSpan);

                            return GLSLSpanMatch.Matched(span, token, preprocessorSpan, TOKEN_TAG);
                        }
                    }
                }
            }

            return GLSLSpanMatch.Unmatched();
        }
    }
}

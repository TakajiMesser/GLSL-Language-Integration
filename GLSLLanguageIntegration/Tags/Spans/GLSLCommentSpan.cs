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
    public static class GLSLCommentSpan
    {
        public const GLSLTokenTypes TOKEN_TAG = GLSLTokenTypes.Comment;

        private static List<SnapshotSpan> _singleLineComments = new List<SnapshotSpan>();
        private static List<SnapshotSpan> _multiLineComments = new List<SnapshotSpan>();

        private static SpanBuilder _singleLineCommentBuilder = new SpanBuilder();
        private static SpanBuilder _multiLineCommentBuilder = new SpanBuilder();

        public static GLSLSpanMatch Match(string token, int position, SnapshotSpan span)
        {
            _singleLineCommentBuilder.Snapshot = span.Snapshot;
            _multiLineCommentBuilder.Snapshot = span.Snapshot;

            if (token.Contains("//"))
            {
                int tokenIndex = token.IndexOf("//");
                int start = position - token.Length + tokenIndex;

                if (!_singleLineCommentBuilder.Start.HasValue)
                {
                    _singleLineCommentBuilder.Start = start;
                    string text = span.GetText();

                    for (var i = start - span.Start.Position; i < text.Length; i++)
                    {
                        char character = text[i];

                        // Keep consuming characters until we find a line end
                        if (character == '\r' || character == '\n')
                        {
                            _singleLineCommentBuilder.End = start + i;
                            var commentSpan = _singleLineCommentBuilder.ToSpan();

                            _singleLineCommentBuilder.Clear();
                            _singleLineComments.Add(commentSpan);

                            return GLSLSpanMatch.Matched(span, token, commentSpan, TOKEN_TAG);
                        }
                    }
                }
            }
            /*else if (token.Contains("/*"))
            {
                int tokenIndex = token.IndexOf("/*");

                var openSpan = _spans.FirstOrDefault(s => s.Length == 0);
                if (openSpan != null)
                {
                    //_spans.Add(new SnapshotSpan(openSpan.Snapshot, openSpan.Start, 0));
                }
                else
                {
                    _spans.Add(new SnapshotSpan(span.Snapshot, position + tokenIndex, 0));
                }
            }
            else if (token.Contains("*"))
            {
                int tokenIndex = token.IndexOf("*");

                var openSpan = _spans.FirstOrDefault(s => s.Length == 0);
                if (openSpan != null)
                {
                    _spans.Remove(openSpan);
                    _spans.Add(new SnapshotSpan(openSpan.Snapshot, openSpan.Start, position + tokenIndex - openSpan.Start));

                    return GLSLSpanMatch.Matched(span, TOKEN_TAG, openSpan.Start, position + tokenIndex - openSpan.Start, 1, 0);
                }
                else
                {
                    //_spans.Add(new SnapshotSpan(span.Snapshot, position + tokenIndex, 0));
                }
            }*/

            return GLSLSpanMatch.Unmatched();
        }
    }
}

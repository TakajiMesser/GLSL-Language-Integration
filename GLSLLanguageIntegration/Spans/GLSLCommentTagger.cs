using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Tags;
using GLSLLanguageIntegration.Tags.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GLSLLanguageIntegration.Spans
{
    public class GLSLCommentTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Comment;

        private List<SnapshotSpan> _singleLineComments = new List<SnapshotSpan>();
        private List<SnapshotSpan> _multiLineComments = new List<SnapshotSpan>();

        private SpanBuilder _singleLineCommentBuilder = new SpanBuilder();
        private SpanBuilder _multiLineCommentBuilder = new SpanBuilder();

        public GLSLSpanResult Match(string token, int position, SnapshotSpan span)
        {
            _singleLineCommentBuilder.Snapshot = span.Snapshot;
            _multiLineCommentBuilder.Snapshot = span.Snapshot;

            var result = new GLSLSpanResult(TOKEN_TYPE, span);
            result.AddSpans(_singleLineComments);
            result.AddSpans(_multiLineComments);

            if (token.Contains("//"))
            {
                int tokenIndex = token.IndexOf("//");
                int start = position - token.Length + tokenIndex;

                if (!_singleLineCommentBuilder.Start.HasValue)
                {
                    _singleLineCommentBuilder.Start = start;
                    string text = span.Snapshot.GetText();

                    for (var i = start; i < text.Length; i++)
                    {
                        char character = text[i];

                        // Keep consuming characters until we find a line end
                        if (character == '\r' || character == '\n')
                        {
                            _singleLineCommentBuilder.End = i;
                            var commentSpan = _singleLineCommentBuilder.ToSpan();

                            _singleLineCommentBuilder.Clear();
                            _singleLineComments.Add(commentSpan);

                            result.Consumed = token.Length + commentSpan.Length;
                            result.AddSpan(commentSpan);
                            break;
                        }
                    }
                }
            }

            if (token.Contains("/*"))
            {
                int tokenIndex = token.IndexOf("/*");
                int start = position - token.Length + tokenIndex;

                if (!_multiLineCommentBuilder.Start.HasValue)
                {
                    _multiLineCommentBuilder.Start = start;
                    string text = span.Snapshot.GetText();

                    for (var i = start; i < text.Length; i++)
                    {
                        char character = text[i];

                        // Keep consuming characters until we find a closing comment
                        if (character == '/' && i > start && text[i - 1] == '*')
                        {
                            _multiLineCommentBuilder.End = i;
                            var commentSpan = _multiLineCommentBuilder.ToSpan();

                            _multiLineCommentBuilder.Clear();
                            _multiLineComments.Add(commentSpan);

                            result.Consumed = token.Length + commentSpan.Length;
                            result.AddSpan(commentSpan);
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }
}

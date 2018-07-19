using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Outlining;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLCommentTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Comment;

        private List<TagSpan<IGLSLTag>> _singleLineComments = new List<TagSpan<IGLSLTag>>();
        private List<TagSpan<IGLSLTag>> _multiLineComments = new List<TagSpan<IGLSLTag>>();

        private SpanBuilder _singleLineCommentBuilder = new SpanBuilder();
        private SpanBuilder _multiLineCommentBuilder = new SpanBuilder();

        public GLSLSpanResult Match(string token, int position, SnapshotSpan span)
        {
            _singleLineCommentBuilder.Snapshot = span.Snapshot;
            _multiLineCommentBuilder.Snapshot = span.Snapshot;

            var result = new GLSLSpanResult(TOKEN_TYPE, span);

            if (token.Contains("//"))
            {
                int tokenIndex = token.IndexOf("//");
                int start = position - token.Length + tokenIndex;

                if (!_singleLineCommentBuilder.Start.HasValue)
                {
                    _singleLineCommentBuilder.Start = start;
                    string text = span.Snapshot.GetText();

                    int end = _singleLineCommentBuilder.ConsumeUntil(text, start, "\r", "\n");
                    if (end >= 0)
                    {
                        _singleLineCommentBuilder.End = end;
                        var commentSpan = _singleLineCommentBuilder.ToSpan();
                        _singleLineCommentBuilder.Clear();

                        var commentTagSpan = new TagSpan<IGLSLTag>(commentSpan, new GLSLClassifierTag(TOKEN_TYPE));
                        _singleLineComments.Add(commentTagSpan);

                        result.Consumed = commentSpan.Length - token.Length;
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

                    int end = _multiLineCommentBuilder.ConsumeUntil(text, start, "*/");
                    if (end >= 0)
                    {
                        _multiLineCommentBuilder.End = end + 1;
                        var commentSpan = _multiLineCommentBuilder.ToSpan();
                        _multiLineCommentBuilder.Clear();

                        var commentTagSpan = new TagSpan<IGLSLTag>(commentSpan, new GLSLClassifierTag(TOKEN_TYPE));
                        _multiLineComments.Add(commentTagSpan);

                        int outlineEnd = _multiLineCommentBuilder.ConsumeUntil(text, start, "\r", "\n");
                        if (outlineEnd >= 0)
                        {
                            var outlineTagSpan = new TagSpan<IGLSLTag>(commentSpan, new GLSLOutlineTag(TOKEN_TYPE, text.Substring(start, outlineEnd - start) + "...*/"));
                            _multiLineComments.Add(outlineTagSpan);
                        }
                        
                        result.Consumed = commentSpan.Length - token.Length;
                    }
                }
            }

            result.AddSpans(_singleLineComments);
            result.AddSpans(_multiLineComments);

            return result;
        }

        public void Clear()
        {
            _singleLineComments.Clear();
            _multiLineComments.Clear();

            _singleLineCommentBuilder.Clear();
            _multiLineCommentBuilder.Clear();
        }
    }
}

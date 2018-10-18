using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Outlining;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLCommentTagger : IGLSLTagger
    {
        public const GLSLTokenTypes TOKEN_TYPE = GLSLTokenTypes.Comment;

        private List<TagSpan<GLSLClassifierTag>> _singleLineComments = new List<TagSpan<GLSLClassifierTag>>();
        private List<TagSpan<IGLSLTag>> _multiLineComments = new List<TagSpan<IGLSLTag>>();

        private SpanBuilder _singleLineCommentBuilder = new SpanBuilder();
        private SpanBuilder _multiLineCommentBuilder = new SpanBuilder();

        public TokenTagCollection Match(SnapshotSpan span)
        {
            _singleLineCommentBuilder.Snapshot = span.Snapshot;
            _multiLineCommentBuilder.Snapshot = span.Snapshot;

            var tokenTags = new TokenTagCollection(span);
            string token = span.GetText();
            int position = span.Start + token.Length;

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

                        var commentTagSpan = new TagSpan<GLSLClassifierTag>(commentSpan, new GLSLClassifierTag(TOKEN_TYPE));
                        _singleLineComments.Add(commentTagSpan);

                        tokenTags.Consumed = commentSpan.Length - token.Length;
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

                        tokenTags.Consumed = commentSpan.Length - token.Length;
                    }
                }
            }

            tokenTags.SetClassifierTag(_singleLineComments);
            tokenTags.SetClassifierTag(_multiLineComments.OfType<TagSpan<GLSLClassifierTag>>());
            tokenTags.AddOutlineTagSpans(_multiLineComments.OfType<TagSpan<GLSLOutlineTag>>());

            return tokenTags;
        }

        public void Translate(ITextSnapshot textSnapshot)
        {
            for (var i = 0; i < _singleLineComments.Count; i++)
            {
                var tagSpan = _singleLineComments[i];
                _singleLineComments[i] = tagSpan.Translated(textSnapshot);
            }

            for (var i = 0; i < _multiLineComments.Count; i++)
            {
                var tagSpan = _multiLineComments[i];
                _multiLineComments[i] = tagSpan.Translated(textSnapshot);
            }
        }

        public void Remove(Span span)
        {
            for (var i = _singleLineComments.Count - 1; i >= 0; i--)
            {
                if (_singleLineComments[i].Span.OverlapsWith(span))
                {
                    _singleLineComments.RemoveAt(i);
                }
            }

            for (var i = _multiLineComments.Count - 1; i >= 0; i--)
            {
                if (_multiLineComments[i].Span.OverlapsWith(span))
                {
                    _multiLineComments.RemoveAt(i);
                }
            }
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

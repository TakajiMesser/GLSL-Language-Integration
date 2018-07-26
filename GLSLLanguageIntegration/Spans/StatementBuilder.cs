using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Spans
{
    public class StatementBuilder
    {
        public int StartPosition { get; private set; }
        public int EndPosition => _tokens.LastOrDefault().EndPosition;
        public int Length => EndPosition - StartPosition;
        public int TokenCount => _tokens.Count;

        public ITextSnapshot Snapshot { get; set; }
        public SnapshotSpan? Span => new SnapshotSpan(Snapshot, StartPosition, Length);

        public IEnumerable<TokenResult> Tokens => _tokens;

        private List<TokenResult> _tokens = new List<TokenResult>();

        public TokenResult GetTokenAt(int index) => _tokens[index];

        public void AppendToken(string token, int position, SnapshotSpan span)
        {
            if (_tokens.Count == 0)
            {
                StartPosition = position - token.Length;
            }

            _tokens.Add(new TokenResult(token, position - token.Length)
            {
                Span = span
            });
        }

        public void AppendResult(string token, int position, GLSLSpanResult result)
        {
            if (result.TokenType != GLSLTokenTypes.Comment && result.TokenType != GLSLTokenTypes.Preprocessor)
            {
                if (_tokens.Count == 0)
                {
                    StartPosition = position - token.Length;
                }

                _tokens.Add(new TokenResult(token, position - token.Length)
                {
                    TokenType = result.TokenType
                });

                //result.Consumed;
                //result.Span;
                //result.TagSpans;
            }
        }

        public void Append(TagSpan<IGLSLTag> tagSpan)
        {
            switch (tagSpan.Tag.TokenType)
            {
                //case GLSLTokenTypes.
            }
        }

        public void Terminate(string token, int position, SnapshotSpan span)
        {
            _tokens.Add(new TokenResult(token, position));
        }

        public void Validate()
        {

        }

        public void Clear() => _tokens.Clear();
    }
}

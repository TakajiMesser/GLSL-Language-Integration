using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Spans
{
    public class StatementBuilder
    {
        public ITextSnapshot Snapshot { get; set; }

        public int StartPosition => _tokens.First().Span.Start;
        public int EndPosition => _tokens.Last().Span.End;
        public int Length => EndPosition - StartPosition;
        public int TokenCount => _tokens.Count;
        public SnapshotSpan? Span => new SnapshotSpan(Snapshot, StartPosition, Length);
        public IEnumerable<TokenResult> Tokens => _tokens;

        private List<TokenResult> _tokens = new List<TokenResult>();

        public TokenResult GetTokenAt(int index) => _tokens[index];

        public void AppendResult(GLSLSpanResult result)
        {
            var tokenResult = new TokenResult(result.Span);

            if (result.IsMatch)
            {
                tokenResult.TokenType = result.TokenType;
            }

            _tokens.Add(tokenResult);
        }

        public void Validate()
        {

        }

        public void Clear() => _tokens.Clear();
    }
}

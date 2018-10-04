using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Spans
{
    public class StatementCollection
    {
        public List<Statement> Statements { get; } = new List<Statement>();
        public List<TagSpan<IGLSLTag>> Preprocessors { get; } = new List<TagSpan<IGLSLTag>>();

        public List<TagSpan<IGLSLTag>> TagSpans => Statements.SelectMany(s => s.TagSpans).Concat(Preprocessors).ToList();

        public void Append(Statement statement)
        {
            Statements.Add(statement);
        }

        public void Append(TagSpan<IGLSLTag> preprocessorTagSpan)
        {
            Preprocessors.Add(preprocessorTagSpan);
        }

        public Statement GetStatementForPosition(int position)
        {
            foreach (var statement in Statements)
            {
                if (statement.Span.Contains(position))
                {
                    return statement;
                }
            }

            return null;
        }

        public void Clear()
        {
            Statements.Clear();
        }
    }
}

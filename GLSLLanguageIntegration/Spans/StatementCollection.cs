using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
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

        public void Replace(Statement statement, int position)
        {
            var existingStatement = Statements.FirstOrDefault(s => s.Span.Start == position);
            if (existingStatement != null)
            {
                Statements.Remove(existingStatement);
            }

            Append(statement);
        }

        public void Append(TagSpan<IGLSLTag> preprocessorTagSpan)
        {
            Preprocessors.Add(preprocessorTagSpan);
        }

        /// <summary>
        /// Purge all statements of any TagSpans that intersect in any way with the text changes
        /// </summary>
        public void PurgeAndUpdate(INormalizedTextChangeCollection textChanges, ITextSnapshot textSnapshot)
        {
            foreach (var statement in Statements)
            {
                statement.PurgeAndUpdate(textChanges, textSnapshot);
            }
        }

        /// <summary>
        /// Returns the best matching statement for the provided position.
        /// If the position is between statements, returns the latter statement.
        /// </summary>
        public Statement GetStatementForPosition(int position)
        {
            foreach (var statement in Statements)
            {
                if (position < statement.Span.End)
                {
                    if (position < statement.Span.Start)
                    {
                        statement.Prepend(statement.Span.Start - position);
                    }

                    return statement;
                }
            }

            return null;
        }

        public TagSpan<IGLSLTag> GetPreprocessorForPosition(int position)
        {
            foreach (var preprocessor in Preprocessors)
            {

            }

            return null;
        }

        public void Clear()
        {
            Statements.Clear();
        }
    }
}

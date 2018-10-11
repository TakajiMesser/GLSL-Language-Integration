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

        public void Purge(INormalizedTextChangeCollection textChanges, ITextSnapshot textSnapshot)
        {
            // Returns any tagspans from our original set (before the text changed) that intersects in any way with our text changes
            // Purge all statements of any TagSpans that intersect in any way with the text changes
            foreach (var statement in Statements)
            {
                var fullSpan = new Span(textChanges.First().NewSpan.Start, textChanges.Last().NewSpan.End);

                for (var i = statement.TagSpans.Count - 1; i >= 0; i--)
                {
                    var tagSpan = statement.TagSpans[i];
                    var translatedSpan = tagSpan.Span.TranslateTo(textSnapshot, SpanTrackingMode.EdgeExclusive);

                    if (translatedSpan.IntersectsWith(fullSpan))
                    {
                        // Now check more incrementally
                        foreach (var textChange in textChanges)
                        {
                            if (translatedSpan.IntersectsWith(textChange.NewSpan))
                            {
                                // Remove this tagspan!
                                statement.TagSpans.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the best matching statement for the provided position.
        /// If the position is between statements, returns the latter statement.
        /// </summary>
        public Statement GetStatementForPosition(int position)
        {
            Statement statementMatch = null;

            foreach (var statement in Statements)
            {
                if (statementMatch != null)
                {
                    statement.Shift(1);
                }
                else if (position < statement.Span.End)
                {
                    if (position < statement.Span.Start)
                    {
                        statement.Prepend(statement.Span.Start - position);
                    }
                    else
                    {
                        statement.Extend(1);
                    }

                    statementMatch = statement;
                }
            }

            return statementMatch;
        }

        public void Clear()
        {
            Statements.Clear();
        }
    }
}

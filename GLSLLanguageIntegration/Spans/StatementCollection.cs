using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
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
            var lastStatement = Statements.LastOrDefault();

            if (lastStatement == null || statement.Span.Start > lastStatement.Span.End.Position)
            {
                Statements.Add(statement);
            }
            else
            {
                for (var i = 0; i < Statements.Count; i++)
                {
                    if (statement.Span.Start < Statements[i].Span.Start)
                    {
                        Statements.Insert(i, statement);
                        break;
                    }
                }
            }
        }

        public void Append(TagSpan<IGLSLTag> preprocessorTagSpan)
        {
            var lastPreprocessor = Preprocessors.LastOrDefault();

            if (lastPreprocessor == null || preprocessorTagSpan.Span.Start > lastPreprocessor.Span.End.Position)
            {
                Preprocessors.Add(preprocessorTagSpan);
            }
            else
            {
                for (var i = 0; i < Preprocessors.Count; i++)
                {
                    if (preprocessorTagSpan.Span.Start < Preprocessors[i].Span.Start)
                    {
                        Preprocessors.Insert(i, preprocessorTagSpan);
                        break;
                    }
                }
            }
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

        public void Translate(ITextSnapshot textSnapshot)
        {
            foreach (var statement in Statements)
            {
                statement.Translate(textSnapshot);
            }

            for (var i = 0; i < Preprocessors.Count; i++)
            {
                var tagSpan = Preprocessors[i];
                Preprocessors[i] = tagSpan.Translated(textSnapshot);
            }
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

            var fullChangeSpan = new Span(textChanges.First().NewSpan.Start, textChanges.Last().NewSpan.End);

            for (var i = Preprocessors.Count - 1; i >= 0; i--)
            {
                var tagSpan = Preprocessors[i];
                tagSpan.Translate(textSnapshot);

                if (tagSpan.Span.IntersectsWith(fullChangeSpan))
                {
                    // Now check more incrementally
                    if (textChanges.Any(t => tagSpan.Span.IntersectsWith(t.NewSpan)))
                    {
                        // Remove this tagspan!
                        Preprocessors.RemoveAt(i);
                    }
                }
            }
        }

        public Span Reprocess(int start, int end)
        {
            var preprocessorSpan = ReprocessPreprocessors(start, end);
            var statementSpan = ReprocessStatements(start, end);

            return Span.FromBounds(Math.Min(preprocessorSpan.Start, statementSpan.Start), Math.Max(preprocessorSpan.End, statementSpan.End));
        }

        private Span ReprocessPreprocessors(int start, int end)
        {
            var nPreprocessorsToRemove = 0;

            for (var i = 0; i < Preprocessors.Count; i++)
            {
                if (start < Preprocessors[i].Span.End)
                {
                    if (start > Preprocessors[i].Span.Start)
                    {
                        start = Preprocessors[i].Span.Start;
                    }
                    else if (end > Preprocessors[i].Span.End)
                    {
                        nPreprocessorsToRemove++;
                    }

                    if (end < Preprocessors[i].Span.End)
                    {
                        end = Preprocessors[i].Span.End;
                        nPreprocessorsToRemove++;
                        Preprocessors.RemoveRange(i - nPreprocessorsToRemove + 1, nPreprocessorsToRemove);

                        return Span.FromBounds(start, end);
                    }
                }
            }

            Preprocessors.RemoveRange(Preprocessors.Count - 1 - nPreprocessorsToRemove, nPreprocessorsToRemove);

            return Span.FromBounds(start, end);
        }

        private Span ReprocessStatements(int start, int end)
        {
            var nStatementsToRemove = 0;

            for (var i = 0; i < Statements.Count; i++)
            {
                if (start < Statements[i].Span.End)
                {
                    if (start > Statements[i].Span.Start)
                    {
                        start = Statements[i].Span.Start;
                    }
                    else if (end > Statements[i].Span.End)
                    {
                        nStatementsToRemove++;
                    }

                    if (end < Statements[i].Span.End)
                    {
                        end = Statements[i].Span.End;
                        nStatementsToRemove++;
                        Statements.RemoveRange(i - nStatementsToRemove + 1, nStatementsToRemove);

                        return Span.FromBounds(start, end);
                    }
                }
            }

            Statements.RemoveRange(Statements.Count - 1 - nStatementsToRemove, nStatementsToRemove);

            return Span.FromBounds(start, end);
        }

        public int GetStatementStart(int position)
        {
            for (var i = 0; i < Statements.Count; i++)
            {
                if (Statements[i].Span.Contains(position))
                {
                    var statementStart = Statements[i].Span.Start.Position;
                    Statements.RemoveAt(i);
                    return statementStart;
                }
            }

            for (var i = 0; i < Preprocessors.Count; i++)
            {
                if (Preprocessors[i].Span.Contains(position))
                {
                    var statementStart = Preprocessors[i].Span.Start.Position;
                    Preprocessors.RemoveAt(i);
                    return statementStart;
                }
            }

            return position;
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
                if (preprocessor.Span.Contains(position))
                {
                    return preprocessor;
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

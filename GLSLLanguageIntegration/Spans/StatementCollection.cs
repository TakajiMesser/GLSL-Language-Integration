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

        public Span Reprocess(int start, int end)
        {
            var preprocessorSpan = ReprocessPreprocessors(start, end);
            var statementSpan = ReprocessStatements(start, end);

            return Span.FromBounds(Math.Min(preprocessorSpan.Start, statementSpan.Start), Math.Max(preprocessorSpan.End, statementSpan.End));
        }

        // TODO - Determine how to handle reprocessing Preprocessors
        // 1) Do I even want to store Preprocessors here? Maybe they should be left stored in the CommentTagger and DirectiveTagger
        // 2) Unlike Statements, we DO NOT need to consider ourselves as part of the next available Preprocessor
            // SO, we only need to remove Preprocessors that overlap with our range, and we should have these Preprocessors affect our return Span
        public Span ReprocessPreprocessors(int start, int end)
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

        // TODO - Determine how to handle Statements that are removed
        // The issue is that what Statements we remove ALSO affects the other Taggers and what they carry
        // What am I actually doing in this method?
        // Given a range, I am going to:
            // 1) Remove statements that fall within this range
            // 2) Remove statements that overlap with this range, and expand our range to cover this overlap
            // 3) Remove the statement that directly follows our range, and expand our range to cover this statement
        // I need to return:
            // 1) The expanded Span range
        public Span ReprocessStatements(int start, int end)
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

        public void Clear()
        {
            Statements.Clear();
        }
    }
}

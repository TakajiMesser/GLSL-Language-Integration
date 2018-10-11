using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Taggers;
using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GLSLLanguageIntegration.Spans
{
    public class StatementBuilder
    {
        public ITextSnapshot Snapshot { get; }

        public int StartPosition => _tokens.First().Span.Start;
        public int EndPosition => _tokens.Last().Span.End;
        public int Length => EndPosition - StartPosition;
        public int TokenCount => _tokens.Count;
        public SnapshotSpan Span => new SnapshotSpan(Snapshot, StartPosition, Length);

        public IEnumerable<TagSpan<IGLSLTag>> Tokens => _tokens;

        private List<TagSpan<IGLSLTag>> _tokens = new List<TagSpan<IGLSLTag>>();
        private List<TagSpan<IGLSLTag>> _tagSpans = new List<TagSpan<IGLSLTag>>();

        public StatementBuilder(ITextSnapshot snapshot)
        {
            Snapshot = snapshot;
        }

        public TagSpan<IGLSLTag> GetTokenAt(int index) => _tokens[index];

        public void AppendTokenTags(TokenTagCollection tokenTags)
        {
            // Collect all processed TagSpans so that we can associate it with this statement
            _tagSpans.AddRange(tokenTags.TagSpans);

            if (tokenTags.Span.GetText() != "}")//tokenTags.ClassifierTagSpan == null || tokenTags.ClassifierTagSpan.Tag.TokenType != GLSLTokenTypes.CurlyBracket)
            {
                var tokenType = tokenTags.ClassifierTagSpan != null
                    ? tokenTags.ClassifierTagSpan.Tag.TokenType
                    : GLSLTokenTypes.None;

                _tokens.Add(new TagSpan<IGLSLTag>(tokenTags.Span, new GLSLClassifierTag(tokenType)));
            }
        }

        // TODO - Still need to move parameter variables to their child scope. They are currently defined at the parent method scope, which is technically incorrect
        public Statement ProcessStatement(GLSLBracketTagger bracketTagger, GLSLFunctionTagger functionTagger, GLSLVariableTagger variableTagger)
        {
            var statement = new Statement(Span);
            statement.TagSpans.AddRange(_tagSpans);

            // Process the constructed statement
            bool isFunctionDefinition = false;

            for (var i = 0; i < TokenCount; i++)
            {
                var token = GetTokenAt(i);

                if (token.Tag.TokenType == GLSLTokenTypes.None)
                {
                    if (i > 0)
                    {
                        var previousToken = GetTokenAt(i - 1);
                        if (previousToken.Tag.TokenType == GLSLTokenTypes.Type)
                        {
                            // We need to determine the scope of this variable
                            var scope = bracketTagger.GetScope(previousToken.Span);

                            // This could be a variable OR a function definition.
                            // For it to be a function, the scope must be zero AND the token must be followed by parentheses
                            if (i > 1)
                            {
                                // This is a variable. Check for preceding keyword
                                if (isFunctionDefinition)
                                {
                                    // In this case, we KNOW that this should be a parameter variable. Look ahead until we get the bracket and find the child scope
                                    //scope = bracketTagger.GetScope(_tokens.Last().Span);
                                }

                                var tokenType = GetVariableType(i, isFunctionDefinition);
                                statement.TagSpans.AddRange(variableTagger.AddToken(token.Span, scope, previousToken.Span.GetText(), tokenType).TagSpans);
                            }
                            else if (scope.Level == 0 && i < TokenCount - 1 && GetTokenAt(i + 1).Span.GetText() == "(")
                            {
                                // We can now confirm that this is a function definition. Any variables defined within the definition are now parameters
                                isFunctionDefinition = true;
                                statement.TagSpans.AddRange(functionTagger.AddToken(token.Span, previousToken.Span.GetText()).TagSpans);
                            }
                            else
                            {
                                statement.TagSpans.AddRange(variableTagger.AddToken(token.Span, scope, previousToken.Span.GetText(), GLSLTokenTypes.LocalVariable).TagSpans);
                            }
                        }
                    }
                }
            }

            Clear();
            return statement;
        }

        private GLSLTokenTypes GetVariableType(int iteration, bool isFunctionDefinition)
        {
            if (isFunctionDefinition)
            {
                return GLSLTokenTypes.ParameterVariable;
            }
            else
            {
                var previousPreviousToken = GetTokenAt(iteration - 2);
                if (previousPreviousToken.Tag.TokenType == GLSLTokenTypes.Keyword)
                {
                    switch (previousPreviousToken.Span.GetText())
                    {
                        case "uniform":
                            return GLSLTokenTypes.UniformVariable;
                        case "in":
                            return GLSLTokenTypes.InputVariable;
                        case "out":
                            return GLSLTokenTypes.OutputVariable;
                    }
                }
            }

            return GLSLTokenTypes.LocalVariable;
        }

        public void Clear()
        {
            _tokens.Clear();
            _tagSpans.Clear();
        }

        /*switch (character)
        {
            case var value when char.IsWhiteSpace(character):
            case ')':
            case '}':
            case ',':
                break;
            case '.':
                // Need to confirm that what came before is a valid variable/identifier
                break;
            case '(':
                // Need to confirm that what came before is a valid function, or certain keywords (e.g. layout)
                var bracketResult = _bracketTagger.Match(new SnapshotSpan(_buffer.CurrentSnapshot, position, 1));
                _statementBuilder.AppendResult(bracketResult);
                yield return bracketResult;
                break;
            case '{':
                // Need to process statement before brackets (for block statements, or for function definitions)
                foreach (var result in ProcessStatement(character, position))
                {
                    yield return result;
                }

                yield return _bracketTagger.Match(new SnapshotSpan(_buffer.CurrentSnapshot, position, 1));
                break;
            case '[':
                // Need to confirm that what came before is a valid function, or certain keywords (e.g. layout)
                yield return _bracketTagger.Match(new SnapshotSpan(_buffer.CurrentSnapshot, position, 1));
                break;
            case ';':
                // End of statement -> Need to check statement for errors
                foreach (var result in ProcessStatement(character, position))
                {
                    yield return result;
                }
                break;
        }*/
    }
}

using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Taggers;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;
using System.IO;
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
        public IEnumerable<TagSpan<IGLSLTag>> Tokens => _tokens;

        private List<TagSpan<IGLSLTag>> _tokens = new List<TagSpan<IGLSLTag>>();

        public TagSpan<IGLSLTag> GetTokenAt(int index) => _tokens[index];

        public void AppendResult(SpanResult result)
        {
            if (result.TokenType != GLSLTokenTypes.CurlyBracket)
            {
                var tokenType = result.TagSpans.Count > 0
                ? result.TokenType
                : GLSLTokenTypes.None;

                var token = new TagSpan<IGLSLTag>(result.Span, new GLSLClassifierTag(tokenType));
                _tokens.Add(token);
            }
        }

        // TODO - Still need to move parameter variables to their child scope. They are currently defined at the parent method scope, which is technically incorrect
        public IEnumerable<SpanResult> ProcessStatement(GLSLBracketTagger bracketTagger, GLSLFunctionTagger functionTagger, GLSLVariableTagger variableTagger)
        {
            //var tokenScope = bracketTagger.GetScope(_tokens.First().Span);
            //File.AppendAllLines(@"C:\Users\Takaji\Desktop\testlog.txt", new[] { tokenScope.Level + " - " + string.Join(" ", _tokens.Select(t => t.Span.GetText())) });

            if (Length > 0)
            {
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

                                // This is a variable. Check for preceding keyword
                                if (i > 1)
                                {
                                    var tokenType = GetVariableType(i, isFunctionDefinition);
                                    yield return variableTagger.AddToken(token.Span, scope, previousToken.Span.GetText(), tokenType);
                                }
                                else
                                {
                                    // In this case, this could be a variable OR a function definition
                                    // For it to be a function, the scope must be zero AND the token must be followed by parentheses
                                    if (scope.Level == 0 && i < TokenCount - 1 && GetTokenAt(i + 1).Span.GetText() == "(")
                                    {
                                        // We can now confirm that this is a function definition. Any variables defined within the definition are now parameters
                                        isFunctionDefinition = true;
                                        yield return functionTagger.AddToken(token.Span, previousToken.Span.GetText());
                                    }
                                    else
                                    {
                                        yield return variableTagger.AddToken(token.Span, scope, previousToken.Span.GetText(), GLSLTokenTypes.LocalVariable);
                                    }

                                }
                            }
                        }
                    }
                }

                Clear();
            }
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
            //File.WriteAllText(@"C:\Users\Takaji\Desktop\testlog.txt", "");
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

using GLSLLanguageIntegration.Taggers;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using System;
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

        // We still have two problems!
        //      1) Parameter variables are in the parent scope like the function, NOT within the child scope of the method like it should be
        //      2) Local variables that are in the same scope level, but in DIFFERENT child scopes are not working as separate variables, for whatever reason

        public IEnumerable<GLSLSpanResult> ProcessStatement(GLSLBracketTagger bracketTagger, GLSLFunctionTagger functionTagger, GLSLVariableTagger variableTagger)
        {
            var tokenScope = bracketTagger.GetScope(_tokens.First().Span);

            if (Length > 0)
            {
                // Process the constructed statement
                //_statementBuilder.Terminate(character.ToString(), position + 1, new SnapshotSpan(_buffer.CurrentSnapshot, position, 1));
                bool isFunctionDefinition = false;

                for (var i = 0; i < TokenCount; i++)
                {
                    var tokenResult = GetTokenAt(i);

                    if (!tokenResult.TokenType.HasValue)
                    {
                        if (i > 0)
                        {
                            var previousResult = GetTokenAt(i - 1);
                            if (previousResult.TokenType == GLSLTokenTypes.Type)
                            {
                                // We need to determine the scope of this variable
                                var scope = bracketTagger.GetScope(tokenResult.Span);

                                // This is a variable. Check for preceding keyword
                                if (i > 1)
                                {
                                    var tokenType = GetVariableType(i, isFunctionDefinition);
                                    yield return variableTagger.AddToken(tokenResult.Token, scope, previousResult.Token, tokenResult.Span.End, tokenResult.Span, tokenType);
                                }
                                else
                                {
                                    // In this case, this could be a variable OR a function definition
                                    // For it to be a function, the scope must be zero AND the token must be followed by parentheses
                                    if (scope.Level == 0 && i < TokenCount - 1)
                                    {
                                        var nextResult = GetTokenAt(i + 1);
                                        if (nextResult.Token == "(")
                                        {
                                            // We can now confirm that this is a function definition. Any variables defined within the definition are now parameters
                                            isFunctionDefinition = true;
                                            yield return functionTagger.AddToken(tokenResult.Token, previousResult.Token, tokenResult.Span.End, tokenResult.Span);
                                        }
                                        else
                                        {
                                            yield return variableTagger.AddToken(tokenResult.Token, scope, previousResult.Token, tokenResult.Span.End, tokenResult.Span, GLSLTokenTypes.LocalVariable);
                                        }
                                    }
                                    else
                                    {
                                        yield return variableTagger.AddToken(tokenResult.Token, scope, previousResult.Token, tokenResult.Span.End, tokenResult.Span, GLSLTokenTypes.LocalVariable);
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
                var previousPreviousResult = GetTokenAt(iteration - 2);
                if (previousPreviousResult.TokenType == GLSLTokenTypes.Keyword)
                {
                    switch (previousPreviousResult.Token)
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
        }
    }
}

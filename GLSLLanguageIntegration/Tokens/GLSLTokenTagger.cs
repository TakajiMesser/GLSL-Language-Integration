using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Taggers;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace GLSLLanguageIntegration.Tokens
{
    internal sealed class GLSLTokenTagger : ITagger<IGLSLTag>
    {
        private ITextBuffer _buffer;
        private SnapshotSpan? _tokenSpan = null;
        private StatementBuilder _statementBuilder = new StatementBuilder();
        private GLSLTagSpanCollection _tagSpans = new GLSLTagSpanCollection();

        private GLSLPreprocessorTagger _preprocessorTagger = new GLSLPreprocessorTagger();
        private GLSLCommentTagger _commentTagger = new GLSLCommentTagger();
        private GLSLKeywordTagger _keywordTagger = new GLSLKeywordTagger();
        private GLSLTypeTagger _typeTagger = new GLSLTypeTagger();
        private GLSLVariableTagger _variableTagger = new GLSLVariableTagger();
        private GLSLStructTagger _structTagger = new GLSLStructTagger();
        private GLSLConstantTagger _constantTagger = new GLSLConstantTagger();
        private GLSLFunctionTagger _functionTagger = new GLSLFunctionTagger();
        private GLSLOperatorTagger _operatorTagger = new GLSLOperatorTagger();
        private GLSLStatementTagger _statementTagger = new GLSLStatementTagger();
        private GLSLBracketTagger _bracketTagger = new GLSLBracketTagger();

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        internal GLSLTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _buffer.Changed += (s, args) =>
            {
                if (args.After == buffer.CurrentSnapshot)
                {
                    // TODO - Avoid re-parsing the entire buffer by only parsing the relevant spans
                    ParseBuffer();// args.Changes);
                }
            };

            _tagSpans.TagsChanged += (s, args) =>
            {
                TagsChanged?.Invoke(this, args);
            };

            ParseBuffer();
        }

        public IEnumerable<ITagSpan<IGLSLTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count > 0)
            {
                foreach (var tagSpan in _tagSpans.GetOverlapping(spans, _buffer.CurrentSnapshot))
                {
                    yield return tagSpan;
                }
            }
        }

        public IEnumerable<Completion> GetCompletions(SnapshotSpan span)
        {
            var scope = _bracketTagger.GetScope(span);
            var token = span.GetText();

            // We need to filter OUT any variables whose token text does not match our current span text

            foreach (var variableInfo in _variableTagger.GetVariables(scope))
            {
                if (variableInfo.Token.Contains(token, StringComparison.OrdinalIgnoreCase))
                {
                    var iconSource = variableInfo.GetImageSource();
                    yield return new Completion(variableInfo.Token, variableInfo.Token, variableInfo.Definition, iconSource, "");
                }
            }

            yield break;
        }

        public object GetQuickInfo(SnapshotSpan span, GLSLTokenTypes tokenType)
        {
            string token = span.GetText();

            switch (tokenType)
            {
                case GLSLTokenTypes.Keyword:
                    return _keywordTagger.GetQuickInfo(token);
                case GLSLTokenTypes.Type:
                    return _typeTagger.GetQuickInfo(token);
                case GLSLTokenTypes.BuiltInVariable:
                case GLSLTokenTypes.InputVariable:
                case GLSLTokenTypes.OutputVariable:
                case GLSLTokenTypes.UniformVariable:
                case GLSLTokenTypes.BufferVariable:
                case GLSLTokenTypes.SharedVariable:
                case GLSLTokenTypes.LocalVariable:
                    var scope = _bracketTagger.GetScope(span);
                    return _variableTagger.GetQuickInfo(token, scope);
                case GLSLTokenTypes.BuiltInFunction:
                case GLSLTokenTypes.Function:
                    return _functionTagger.GetQuickInfo(token);
                case GLSLTokenTypes.BuiltInConstant:
                    return _constantTagger.GetQuickInfo(token);
                case GLSLTokenTypes.Preprocessor:
                    return _preprocessorTagger.GetQuickInfo(token);
            }

            return null;
        }

        private void ParseBuffer()
        {
            ClearTaggers();
            _tokenSpan = null;
            _statementBuilder.Clear();

            var textSnapshot = _buffer.CurrentSnapshot;
            _statementBuilder.Snapshot = textSnapshot;

            var text = textSnapshot.GetText();

            var newTagSpans = new List<TagSpan<IGLSLTag>>();

            for (var i = 0; i < text.Length; i++)
            {
                foreach (var result in ProcessCharacter(text[i], i))
                {
                    newTagSpans.AddRange(result.TagSpans);
                    i += result.Consumed;
                }
            }

            _tagSpans.Update(textSnapshot, newTagSpans);
        }

        private void ParseBuffer(INormalizedTextChangeCollection textChanges)
        {
            var textSnapshot = _buffer.CurrentSnapshot;

            //_tokenBuffer.Clear();
            //_tokenBuffer.Snapshot = textSnapshot;

            _tagSpans.GetOverlapping(textChanges, textSnapshot);

            foreach (var textChange in textChanges)
            {
                //textChange.
            }

            var text = textSnapshot.GetText();

            var newTagSpans = new List<TagSpan<IGLSLTag>>();

            for (var i = 0; i < text.Length; i++)
            {
                foreach (var result in ProcessCharacter(text[i], i))
                {
                    newTagSpans.AddRange(result.TagSpans);
                    i += result.Consumed;
                }
            }

            _tagSpans.Update(textSnapshot, newTagSpans);
        }

        private void ClearTaggers()
        {
            _preprocessorTagger.Clear();
            _commentTagger.Clear();
            _keywordTagger.Clear();
            _typeTagger.Clear();
            _variableTagger.Clear();
            _structTagger.Clear();
            _constantTagger.Clear();
            _functionTagger.Clear();
            _operatorTagger.Clear();
            _statementTagger.Clear();
            _bracketTagger.Clear();
        }

        private IEnumerable<GLSLSpanResult> ProcessCharacter(char character, int position)
        {
            switch (character)
            {
                case var value when char.IsWhiteSpace(character):
                case ')':
                case '}':
                case ',':
                    yield return ProcessBuffer(position);
                    break;
                case '.':
                    // Need to confirm that what came before is a valid variable/identifier
                    yield return ProcessBuffer(position);
                    break;
                case '(':
                    // Need to confirm that what came before is a valid function, or certain keywords (e.g. layout)
                    yield return ProcessBuffer(position);
                    var bracketResult = _bracketTagger.Match(new SnapshotSpan(_buffer.CurrentSnapshot, position, 1));
                    _statementBuilder.AppendResult(bracketResult);
                    yield return bracketResult;
                    break;
                case '{':
                    yield return ProcessBuffer(position);
                    yield return _bracketTagger.Match(new SnapshotSpan(_buffer.CurrentSnapshot, position, 1));
                    break;
                case '[':
                    // Need to confirm that what came before is a valid function, or certain keywords (e.g. layout)
                    yield return ProcessBuffer(position);
                    yield return _bracketTagger.Match(new SnapshotSpan(_buffer.CurrentSnapshot, position, 1));
                    break;
                case ';':
                    // End of statement -> Need to check statement for errors
                    yield return ProcessBuffer(position);
                    foreach (var result in ProcessStatement(character, position))
                    {
                        yield return result;
                    }
                    break;
                default:
                    _tokenSpan = _tokenSpan.HasValue
                        ? _tokenSpan.Value.Extended(1)
                        : new SnapshotSpan(_buffer.CurrentSnapshot, position, 1);
                    break;
            }
        }

        private IEnumerable<GLSLSpanResult> ProcessStatement(char character, int position)
        {
            if (_statementBuilder.Length > 0)
            {
                // Process the constructed statement
                //_statementBuilder.Terminate(character.ToString(), position + 1, new SnapshotSpan(_buffer.CurrentSnapshot, position, 1));

                for (var i = 0; i < _statementBuilder.TokenCount; i++)
                {
                    var tokenResult = _statementBuilder.GetTokenAt(i);

                    if (!tokenResult.TokenType.HasValue)
                    {
                        if (i > 0)
                        {
                            var previousResult = _statementBuilder.GetTokenAt(i - 1);
                            if (previousResult.TokenType == GLSLTokenTypes.Type)
                            {
                                // We need to determine the scope of this variable
                                var scope = _bracketTagger.GetScope(tokenResult.Span);

                                // This is a variable. Check for preceding keyword
                                if (i > 1)
                                {
                                    var previousPreviousResult = _statementBuilder.GetTokenAt(i - 2);
                                    if (previousPreviousResult.TokenType == GLSLTokenTypes.Keyword)
                                    {
                                        switch (previousPreviousResult.Token)
                                        {
                                            case "uniform":
                                                yield return _variableTagger.AddToken(tokenResult.Token, scope, previousResult.Token, tokenResult.Span.End, tokenResult.Span, GLSLTokenTypes.UniformVariable);
                                                break;
                                            case "in":
                                                yield return _variableTagger.AddToken(tokenResult.Token, scope, previousResult.Token, tokenResult.Span.End, tokenResult.Span, GLSLTokenTypes.InputVariable);
                                                break;
                                            case "out":
                                                yield return _variableTagger.AddToken(tokenResult.Token, scope, previousResult.Token, tokenResult.Span.End, tokenResult.Span, GLSLTokenTypes.OutputVariable);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        yield return _variableTagger.AddToken(tokenResult.Token, scope, previousResult.Token, tokenResult.Span.End, tokenResult.Span, GLSLTokenTypes.LocalVariable);
                                    }
                                }
                                else
                                {
                                    // In this case, this could be a variable OR a function definition
                                    // For it to be a function, the scope must be zero AND the token must be followed by parentheses
                                    if (scope.Level == 0 && i < _statementBuilder.TokenCount - 1)
                                    {
                                        var nextResult = _statementBuilder.GetTokenAt(i + 1);
                                        if (nextResult.Token == "(")
                                        {
                                            // We can now confirm that this is a function definition. Any variables defined within the definition are now parameters
                                            yield return _functionTagger.AddToken(tokenResult.Token, previousResult.Token, tokenResult.Span.End, tokenResult.Span);
                                            continue;
                                        }
                                    }

                                    yield return _variableTagger.AddToken(tokenResult.Token, scope, previousResult.Token, tokenResult.Span.End, tokenResult.Span, GLSLTokenTypes.LocalVariable);
                                }
                            }
                        }
                    }
                }

                _statementBuilder.Clear();
            }

            yield break;
        }

        private GLSLSpanResult ProcessBuffer(int position)
        {
            var result = new GLSLSpanResult();

            if (_tokenSpan.HasValue)
            {
                result = Tokenize(_tokenSpan.Value);
                _statementBuilder.AppendResult(result);
                _tokenSpan = null;
            }

            return result;
        }

        private GLSLSpanResult Tokenize(SnapshotSpan span)
        {
            var result = new GLSLSpanResult();

            if (!string.IsNullOrWhiteSpace(span.GetText()))
            {
                result = _commentTagger.Match(span);

                if (!result.IsMatch)
                {
                    result = _preprocessorTagger.Match(span);
                }

                if (!result.IsMatch)
                {
                    result = _keywordTagger.Match(span);
                }

                if (!result.IsMatch)
                {
                    result = _typeTagger.Match(span);
                }

                if (!result.IsMatch)
                {
                    result = _variableTagger.Match(span);
                }

                if (!result.IsMatch)
                {
                    result = _structTagger.Match(span);
                }

                if (!result.IsMatch)
                {
                    result = _constantTagger.Match(span);
                }

                if (!result.IsMatch)
                {
                    result = _functionTagger.Match(span);
                }

                if (!result.IsMatch)
                {
                    result = _operatorTagger.Match(span);
                }

                /*if (!result.IsMatch)
                {
                    result = _statementTagger.Match(token, position, span);
                }*/
            }

            return result;
        }

        /*private IEnumerable<ITagSpan<GLSLTokenTag>> GetTagsForToken(SnapshotSpan span, string token, int position)
        {
            if (token.Length > 0)
            {
                var match = FindMatch(span, Enumerable.Empty<SnapshotSpan>(), token, Enumerable.Empty<string>(), position);

                if (match.IsMatch)
                {
                    yield return match.TagSpan;

                    // Recursively continue checking the remaining subtoken until we have checked every character
                    var subToken = token.Substring(0, match.Start - position);

                    foreach (var tag in GetTagsForToken(span, subToken, position))
                    {
                        yield return tag;
                    }
                }
            }
        }*/
    }
}

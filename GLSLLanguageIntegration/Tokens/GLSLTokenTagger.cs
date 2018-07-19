using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Taggers;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Tokens
{
    internal sealed class GLSLTokenTagger : ITagger<IGLSLTag>
    {
        private ITextBuffer _buffer;
        private TokenBuilder _tokenBuffer = new TokenBuilder();
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

        public object GetQuickInfo(string token, GLSLTokenTypes tokenType)
        {
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
                    return _variableTagger.GetQuickInfo(token);
                case GLSLTokenTypes.BuiltInFunction:
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
            _tokenBuffer.Clear();
            _statementBuilder.Clear();

            var textSnapshot = _buffer.CurrentSnapshot;
            _tokenBuffer.Snapshot = textSnapshot;
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

            _tokenBuffer.Clear();
            _tokenBuffer.Snapshot = textSnapshot;

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
                    yield return ProcessBuffer(position);
                    break;
                case '.':
                    // Need to confirm that what came before is a valid variable/identifier
                    yield return ProcessBuffer(position);
                    break;
                case '(':
                    // Need to confirm that what came before is a valid function, or certain keywords (e.g. layout)
                    yield return ProcessBuffer(position);
                    yield return _bracketTagger.Match(character.ToString(), position + 1, new SnapshotSpan(_tokenBuffer.Snapshot, position, 1));
                    break;
                case '{':
                    yield return ProcessBuffer(position);
                    yield return _bracketTagger.Match(character.ToString(), position + 1, new SnapshotSpan(_tokenBuffer.Snapshot, position, 1));
                    break;
                case '[':
                    // Need to confirm that what came before is a valid function, or certain keywords (e.g. layout)
                    yield return ProcessBuffer(position);
                    yield return _bracketTagger.Match(character.ToString(), position + 1, new SnapshotSpan(_tokenBuffer.Snapshot, position, 1));
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
                    _tokenBuffer.Append(character, position);
                    break;
            }
        }

        private IEnumerable<GLSLSpanResult> ProcessStatement(char character, int position)
        {
            if (_statementBuilder.Length > 0)
            {
                // Process the constructed statement
                _statementBuilder.Terminate(character.ToString(), position + 1, new SnapshotSpan(_tokenBuffer.Snapshot, position, 1));

                for (var i = 0; i < _statementBuilder.TokenCount; i++)
                {
                    var tokenResult = _statementBuilder.GetTokenAt(i);

                    var token = tokenResult.Token;
                    var type = tokenResult.TokenType;
                    var start = tokenResult.StartPosition;
                    var end = tokenResult.EndPosition;

                    if (!tokenResult.TokenType.HasValue)
                    {
                        if (i > 0)
                        {
                            var previousResult = _statementBuilder.GetTokenAt(i - 1);
                            if (previousResult.TokenType == GLSLTokenTypes.Type)
                            {
                                // This is a variable. Check for preceding keyword
                                if (i > 1)
                                {
                                    var previousPreviousResult = _statementBuilder.GetTokenAt(i - 2);
                                    if (previousPreviousResult.TokenType == GLSLTokenTypes.Keyword)
                                    {
                                        switch (previousPreviousResult.Token)
                                        {
                                            case "uniform":
                                                yield return _variableTagger.AddToken(tokenResult.Token, tokenResult.EndPosition, tokenResult.Span, GLSLTokenTypes.UniformVariable);
                                                break;
                                            case "in":
                                                yield return _variableTagger.AddToken(tokenResult.Token, tokenResult.EndPosition, tokenResult.Span, GLSLTokenTypes.InputVariable);
                                                break;
                                            case "out":
                                                yield return _variableTagger.AddToken(tokenResult.Token, tokenResult.EndPosition, tokenResult.Span, GLSLTokenTypes.OutputVariable);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        yield return _variableTagger.AddToken(tokenResult.Token, tokenResult.EndPosition, tokenResult.Span, GLSLTokenTypes.LocalVariable);
                                    }
                                }
                            }
                        }
                    }
                }

                /*foreach (var tokenResult in _statementBuilder.Tokens)
                {
                    var token = tokenResult.Token;
                    var type = tokenResult.TokenType;
                    var start = tokenResult.StartPosition;
                    var end = tokenResult.EndPosition;
                }*/

                _statementBuilder.Clear();
            }

            yield break;
        }

        private GLSLSpanResult ProcessBuffer(int position)
        {
            var result = new GLSLSpanResult();

            if (_tokenBuffer.Length > 0)
            {
                string token = _tokenBuffer.ToString();
                result = Tokenize(token, position, _tokenBuffer.Span);

                if (result.IsMatch)
                {
                    _statementBuilder.AppendResult(token, position, result);
                }
                else
                {
                    _statementBuilder.AppendToken(token, position, _tokenBuffer.Span);
                }
                
                _tokenBuffer.Clear();
            }

            return result;
        }

        private GLSLSpanResult Tokenize(string token, int position, SnapshotSpan span)
        {
            var result = new GLSLSpanResult();

            if (!string.IsNullOrWhiteSpace(token))
            {
                result = _commentTagger.Match(token, position, span);

                if (!result.IsMatch)
                {
                    result = _preprocessorTagger.Match(token, position, span);
                }

                if (!result.IsMatch)
                {
                    result = _keywordTagger.Match(token, position, span);
                }

                if (!result.IsMatch)
                {
                    result = _typeTagger.Match(token, position, span);
                }

                if (!result.IsMatch)
                {
                    result = _variableTagger.Match(token, position, span);
                }

                if (!result.IsMatch)
                {
                    result = _structTagger.Match(token, position, span);
                }

                if (!result.IsMatch)
                {
                    result = _constantTagger.Match(token, position, span);
                }

                if (!result.IsMatch)
                {
                    result = _functionTagger.Match(token, position, span);
                }

                if (!result.IsMatch)
                {
                    result = _operatorTagger.Match(token, position, span);
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

﻿using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Taggers;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Tokens
{
    internal sealed class GLSLTokenTagger : ITagger<IGLSLTag>
    {
        private ITextBuffer _buffer;
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
                case GLSLTokenTypes.ParameterVariable:
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
            _statementBuilder.Clear();

            var textSnapshot = _buffer.CurrentSnapshot;
            _statementBuilder.Snapshot = textSnapshot;

            var tagSpans = new List<TagSpan<IGLSLTag>>();
            var tokenBuilder = new TokenBuilder(_buffer);

            while (!tokenBuilder.IsCompleted)
            {
                var tokenSpan = tokenBuilder.GetNextTokenSpan();

                var spanResult = ProcessToken(tokenSpan);
                tokenBuilder.Position += spanResult.Consumed;

                if (spanResult.TokenType != GLSLTokenTypes.Comment && spanResult.TokenType != GLSLTokenTypes.Preprocessor)
                {
                    var token = tokenSpan.GetText();

                    if (token == "{" || token == "[" || token == "(")
                    {
                        var bracketResult = _bracketTagger.Match(tokenSpan);
                        tagSpans.AddRange(bracketResult.TagSpans);
                    }
                    
                    if (token != "{" && token != "}")
                    {
                        _statementBuilder.AppendResult(spanResult);
                    }

                    if (spanResult.TokenType == GLSLTokenTypes.Semicolon || token == "{")
                    {
                        foreach (var statementResult in _statementBuilder.ProcessStatement(_bracketTagger, _functionTagger, _variableTagger))
                        {
                            tagSpans.AddRange(statementResult.TagSpans);
                        }
                    }
                }

                tagSpans.AddRange(spanResult.TagSpans);
            }

            _tagSpans.Update(textSnapshot, tagSpans);
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
                /*foreach (var result in ProcessCharacter(text[i], i))
                {
                    newTagSpans.AddRange(result.TagSpans);
                    i += result.Consumed;
                }*/
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

        private GLSLSpanResult ProcessToken(SnapshotSpan span)
        {
            var token = span.GetText();
            var result = _commentTagger.Match(span);

            if (!result.IsMatch)
            {
                result = _preprocessorTagger.Match(span);
            }

            if (!result.IsMatch && span.GetText() == ";")
            {
                return new GLSLSpanResult(GLSLTokenTypes.Semicolon, span);
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
                var scope = _bracketTagger.GetScope(span);
                result = _variableTagger.Match(span, scope);
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

            return result;
        }

        public Scope GetScope(SnapshotSpan span) => _bracketTagger.GetScope(span);

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

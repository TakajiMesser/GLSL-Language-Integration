using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Taggers;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Tokens
{
    internal sealed class GLSLTagger : ITagger<IGLSLTag>
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

        internal GLSLTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _buffer.Changed += (s, args) =>
            {
                if (args.After == buffer.CurrentSnapshot)
                {
                    ParseBuffer(args.Changes);
                }
            };

            _tagSpans.TagsChanged += (s, args) => TagsChanged?.Invoke(this, args);

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
                case GLSLTokenTypes.PreprocessorDirective:
                    return _preprocessorTagger.GetQuickInfo(token);
            }

            return null;
        }

        private void ParseBuffer()
        {
            var textSnapshot = _buffer.CurrentSnapshot;
            _statementBuilder.Snapshot = textSnapshot;

            var tagSpans = new List<TagSpan<IGLSLTag>>();
            var tokenBuilder = new TokenBuilder(_buffer);

            while (!tokenBuilder.IsCompleted)
            {
                var tokenSpan = tokenBuilder.GetNextTokenSpan();

                var spanResult = ProcessToken(tokenSpan);
                tagSpans.AddRange(spanResult.TagSpans);

                tokenBuilder.Position += spanResult.Consumed;

                // Skip any preprocessors
                if (!spanResult.TokenType.IsPreprocessor())
                {
                    var token = tokenSpan.GetText();

                    _statementBuilder.AppendResult(spanResult);

                    if (spanResult.TokenType == GLSLTokenTypes.Semicolon || token == "{")
                    {
                        foreach (var statementResult in _statementBuilder.ProcessStatement(_bracketTagger, _functionTagger, _variableTagger))
                        {
                            tagSpans.AddRange(statementResult.TagSpans);
                        }
                    }
                }
            }

            _tagSpans.Update(textSnapshot, tagSpans);
        }

        private void ParseBuffer(INormalizedTextChangeCollection textChanges)
        {
            // TODO - Avoid re-parsing the entire buffer by only parsing the relevant spans
            ClearTaggers();
            _statementBuilder.Clear();
            ParseBuffer();
            return;

            var textSnapshot = _buffer.CurrentSnapshot;
            _statementBuilder.Snapshot = textSnapshot;

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

        private SpanResult ProcessToken(SnapshotSpan span)
        {
            // First check for token types that can consume past the current span (and stops any current processing from continuing)
            var spanResult = _commentTagger.Match(span);
            if (spanResult.TagSpans.Count > 0)
            {
                return spanResult;
            }

            spanResult = _preprocessorTagger.Match(span);
            if (spanResult.TagSpans.Count > 0)
            {
                return spanResult;
            }

            var token = span.GetText();

            if (token == ";")
            {
                return new SpanResult(GLSLTokenTypes.Semicolon, span);
            }

            var bracketResult = _bracketTagger.Match(span);
            if (bracketResult.TagSpans.Count > 0)
            {
                return bracketResult;
            }

            spanResult = GetSpanResult(span);
            // Here we should append from the bracket result by finding overlapping bracket outline spans with this span

            return spanResult;
        }

        private SpanResult GetSpanResult(SnapshotSpan span)
        {
            SpanResult spanResult = _keywordTagger.Match(span);
            if (spanResult.TagSpans.Count > 0)
            {
                return spanResult;
            }

            spanResult = _typeTagger.Match(span);
            if (spanResult.TagSpans.Count > 0)
            {
                return spanResult;
            }

            var scope = _bracketTagger.GetScope(span);
            spanResult = _variableTagger.Match(span, scope);
            if (spanResult.TagSpans.Count > 0)
            {
                return spanResult;
            }

            spanResult = _structTagger.Match(span);
            if (spanResult.TagSpans.Count > 0)
            {
                return spanResult;
            }

            spanResult = _constantTagger.Match(span);
            if (spanResult.TagSpans.Count > 0)
            {
                return spanResult;
            }

            spanResult = _functionTagger.Match(span);
            if (spanResult.TagSpans.Count > 0)
            {
                return spanResult;
            }

            spanResult = _operatorTagger.Match(span);
            if (spanResult.TagSpans.Count > 0)
            {
                return spanResult;
            }

            /*if (!result.IsMatch)
            {
                result = _statementTagger.Match(token, position, span);
            }*/

            return new SpanResult(GLSLTokenTypes.None, span);
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

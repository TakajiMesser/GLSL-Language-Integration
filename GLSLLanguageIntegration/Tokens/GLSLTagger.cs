﻿using GLSLLanguageIntegration.Classification;
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
        private StatementCollection _statements = new StatementCollection();
        private GLSLTagSpanCollection _tagSpans = new GLSLTagSpanCollection();

        private GLSLDirectiveTagger _directiveTagger = new GLSLDirectiveTagger();
        private GLSLCommentTagger _commentTagger = new GLSLCommentTagger();
        private GLSLKeywordTagger _keywordTagger = new GLSLKeywordTagger();
        private GLSLTypeTagger _typeTagger = new GLSLTypeTagger();
        private GLSLVariableTagger _variableTagger = new GLSLVariableTagger();
        private GLSLStructTagger _structTagger = new GLSLStructTagger();
        private GLSLConstantTagger _constantTagger = new GLSLConstantTagger();
        private GLSLFunctionTagger _functionTagger = new GLSLFunctionTagger();
        private GLSLOperatorTagger _operatorTagger = new GLSLOperatorTagger();
        private GLSLBracketTagger _bracketTagger = new GLSLBracketTagger();

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        internal GLSLTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _buffer.Changed += (s, args) =>
            {
                if (args.After == buffer.CurrentSnapshot)
                {
                    // We must feed the GLSLTagSpanCollection ALL tagSpans, old and new, and it will determine what the appropriate differences are
                    ParseBufferChanges(args.Changes, _buffer.CurrentSnapshot);
                }
            };

            _tagSpans.TagsChanged += (s, args) => TagsChanged?.Invoke(this, args);

            ParseBuffer(_buffer.CurrentSnapshot);
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
                case GLSLTokenTypes.Directive:
                    return _directiveTagger.GetQuickInfo(token);
            }

            return null;
        }

        private void ParseBuffer(ITextSnapshot textSnapshot)
        {
            var tokenBuilder = new TokenBuilder(_buffer);
            var statementBuilder = new StatementBuilder(textSnapshot);

            foreach (var tokenSpan in tokenBuilder.GetTokenSpans(0, textSnapshot.Length))
            {
                ProcessTokenSpan(tokenSpan, tokenBuilder, statementBuilder);
            }

            _tagSpans.Update(textSnapshot, _statements.TagSpans);
        }

        private void ParseBufferChanges(INormalizedTextChangeCollection textChanges, ITextSnapshot textSnapshot)
        {
            // We must feed the GLSLTagSpanCollection ALL tagSpans, old and new, and it will determine what the appropriate differences are
            var tokenBuilder = new TokenBuilder(_buffer);
            var statementBuilder = new StatementBuilder(textSnapshot);

            // Returns any tagspans from our original set (before the text changed) that intersects in any way with our text changes
            _statements.Translate(textSnapshot);
            _commentTagger.Translate(textSnapshot);
            //_bracketTagger.Translate(textSnapshot);
            _variableTagger.Translate(textSnapshot);
            _functionTagger.Translate(textSnapshot);

            foreach (var textChange in textChanges)
            {
                // "Reprocess" statement here instead, so we can track which statements and spans we are removing
                var preprocessorSpan = _statements.ReprocessPreprocessors(textChange.NewPosition, textChange.NewEnd);
                _commentTagger.Remove(preprocessorSpan);

                // Need to translate and remove redundant spans from VariableTagger, FunctionTagger
                var statementSpan = _statements.ReprocessStatements(textChange.NewPosition, textChange.NewEnd);
                _variableTagger.Remove(statementSpan);
                _functionTagger.Remove(statementSpan);
                
                var span = Span.FromBounds(Math.Min(preprocessorSpan.Start, statementSpan.Start), Math.Max(preprocessorSpan.End, statementSpan.End));

                // For now, we can just determine to RE-PARSE the entirety of the current statement that we are in
                // This is easy enough, but we ALSO need to remove any tag spans that relate to this statement, since we don't want to end up re-adding those tag spans
                if (tokenBuilder.Position <= span.Start)
                {
                    // Now we generate token spans until
                    //  1) we have parsed at least the full textChange.NewLength amount, and
                    //  2) we finish whatever statement we are currently on
                    foreach (var tokenSpan in tokenBuilder.GetTokenSpans(span.Start, Math.Max(textChange.NewLength + span.Start, span.End)))
                    {
                        ProcessTokenSpan(tokenSpan, tokenBuilder, statementBuilder);
                    }
                }
            }

            _tagSpans.Update(textSnapshot, _statements.TagSpans);
        }

        private void ProcessTokenSpan(SnapshotSpan tokenSpan, TokenBuilder tokenBuilder, StatementBuilder statementBuilder)
        {
            var tokenTags = ProcessToken(tokenSpan);
            tokenBuilder.Position += tokenTags.Consumed;

            // Track preprocessors, but don't process them in our statements
            if (tokenTags.ClassifierTagSpan != null && tokenTags.ClassifierTagSpan.Tag.TokenType.IsPreprocessor())
            {
                var preprocessorTagSpan = new TagSpan<IGLSLTag>(tokenTags.ClassifierTagSpan.Span, tokenTags.ClassifierTagSpan.Tag);
                _statements.Append(preprocessorTagSpan);
            }
            else
            {
                var token = tokenSpan.GetText();
                statementBuilder.AppendTokenTags(tokenTags);

                // Process the statement once we encounter either a ';' or '{' character
                if ((tokenTags.ClassifierTagSpan != null && tokenTags.ClassifierTagSpan.Tag.TokenType == GLSLTokenTypes.Semicolon) || token == "{")
                {
                    var statement = statementBuilder.ProcessStatement(_bracketTagger, _functionTagger, _variableTagger);
                    _statements.Append(statement);
                }
            }
        }

        private TokenTagCollection ProcessToken(SnapshotSpan span)
        {
            // First check for token types that can consume past the current span (and stops any current processing from continuing)
            var tokenTags = _commentTagger.Match(span);
            if (tokenTags.ClassifierTagSpan != null)
            {
                return tokenTags;
            }

            tokenTags = _directiveTagger.Match(span);
            if (tokenTags.ClassifierTagSpan != null)
            {
                return tokenTags;
            }

            var token = span.GetText();

            if (token == ";")
            {
                return new TokenTagCollection(span)
                {
                    ClassifierTagSpan = new TagSpan<GLSLClassifierTag>(span, new GLSLClassifierTag(GLSLTokenTypes.Semicolon))
                };
            }

            var bracketResult = _bracketTagger.Match(span);
            if (bracketResult.ClassifierTagSpan != null)
            {
                return bracketResult;
            }

            tokenTags = GetTokenTags(span);
            // Here we should append from the bracket result by finding overlapping bracket outline spans with this span

            return tokenTags;
        }

        private TokenTagCollection GetTokenTags(SnapshotSpan span)
        {
            TokenTagCollection spanResult = _keywordTagger.Match(span);
            if (spanResult.ClassifierTagSpan != null)
            {
                return spanResult;
            }

            spanResult = _typeTagger.Match(span);
            if (spanResult.ClassifierTagSpan != null)
            {
                return spanResult;
            }

            var scope = _bracketTagger.GetScope(span);
            spanResult = _variableTagger.Match(span, scope);
            if (spanResult.ClassifierTagSpan != null)
            {
                return spanResult;
            }

            spanResult = _structTagger.Match(span);
            if (spanResult.ClassifierTagSpan != null)
            {
                return spanResult;
            }

            spanResult = _constantTagger.Match(span);
            if (spanResult.ClassifierTagSpan != null)
            {
                return spanResult;
            }

            spanResult = _functionTagger.Match(span);
            if (spanResult.ClassifierTagSpan != null)
            {
                return spanResult;
            }

            spanResult = _operatorTagger.Match(span);
            if (spanResult.ClassifierTagSpan != null)
            {
                return spanResult;
            }

            return new TokenTagCollection(span);
        }

        public Scope GetScope(SnapshotSpan span) => _bracketTagger.GetScope(span);

        private void ClearTaggers()
        {
            _directiveTagger.Clear();
            _commentTagger.Clear();
            _keywordTagger.Clear();
            _typeTagger.Clear();
            _variableTagger.Clear();
            _structTagger.Clear();
            _constantTagger.Clear();
            _functionTagger.Clear();
            _operatorTagger.Clear();
            _bracketTagger.Clear();
        }
    }
}

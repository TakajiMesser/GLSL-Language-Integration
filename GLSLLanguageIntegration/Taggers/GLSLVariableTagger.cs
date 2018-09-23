﻿using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLVariableTagger// : IGLSLTagger
    {
        public const GLSLTokenTypes INPUT_TOKEN_TYPE = GLSLTokenTypes.InputVariable;
        public const GLSLTokenTypes OUTPUT_TOKEN_TYPE = GLSLTokenTypes.OutputVariable;
        public const GLSLTokenTypes UNIFORM_TOKEN_TYPE = GLSLTokenTypes.UniformVariable;
        public const GLSLTokenTypes BUFFER_TOKEN_TYPE = GLSLTokenTypes.BufferVariable;
        public const GLSLTokenTypes SHARED_TOKEN_TYPE = GLSLTokenTypes.SharedVariable;
        public const GLSLTokenTypes BUILT_IN_TOKEN_TYPE = GLSLTokenTypes.BuiltInVariable;
        public const GLSLTokenTypes LOCAL_TOKEN_TYPE = GLSLTokenTypes.LocalVariable;

        private List<TagSpan<IGLSLTag>> _inputVariables = new List<TagSpan<IGLSLTag>>();
        private List<TagSpan<IGLSLTag>> _outputVariables = new List<TagSpan<IGLSLTag>>();
        private List<TagSpan<IGLSLTag>> _uniformVariables = new List<TagSpan<IGLSLTag>>();
        private List<TagSpan<IGLSLTag>> _localVariables = new List<TagSpan<IGLSLTag>>();

        //private Dictionary<string, VariableInfo> _variableInfoByToken = new Dictionary<string, VariableInfo>();
        //private List<VariableInfo> _variableInfos = new List<VariableInfo>();
        private VariableInfoCollection _variableInfos = new VariableInfoCollection();

        private TokenSet _tokens = new TokenSet(Resources.Identifiers, BUILT_IN_TOKEN_TYPE);

        public object GetQuickInfo(string token, Scope scope)
        {
            if (_tokens.Contains(token))
            {
                return _tokens.GetInfo(token).ToQuickInfo();
            }
            else
            {
                var variableInfo = _variableInfos.GetVariable(token, scope);
                if (variableInfo != null)
                {
                    return variableInfo.ToQuickInfo();
                }
            }

            return null;
        }

        public IEnumerable<VariableInfo> GetVariables(Scope scope) => _variableInfos.GetVariables(scope);

        public TokenTagCollection AddToken(SnapshotSpan span, Scope scope, string variableType, GLSLTokenTypes tokenType)
        {
            var tokenTags = new TokenTagCollection(span)
            {
                ClassifierTagSpan = new TagSpan<GLSLClassifierTag>(span, new GLSLClassifierTag(tokenType))
            };
            
            switch (tokenType)
            {
                case GLSLTokenTypes.InputVariable:
                    _inputVariables.Add(new TagSpan<IGLSLTag>(span, new GLSLClassifierTag(tokenType)));
                    break;
                case GLSLTokenTypes.OutputVariable:
                    _outputVariables.Add(new TagSpan<IGLSLTag>(span, new GLSLClassifierTag(tokenType)));
                    break;
                case GLSLTokenTypes.UniformVariable:
                    _uniformVariables.Add(new TagSpan<IGLSLTag>(span, new GLSLClassifierTag(tokenType)));
                    break;
                case GLSLTokenTypes.LocalVariable:
                    _localVariables.Add(new TagSpan<IGLSLTag>(span, new GLSLClassifierTag(tokenType)));
                    break;
            }

            var token = span.GetText();
            _variableInfos.Add(new VariableInfo(token, scope, variableType, tokenType));
            //_variableInfoByToken[token] = new VariableInfo(token, scope, variableType, type);

            return tokenTags;
        }

        public TokenTagCollection Match(SnapshotSpan span, Scope scope)
        {
            var tokenTags = new TokenTagCollection(span);

            string token = span.GetText();
            var matchType = MatchTokenType(token, scope);

            if (matchType != GLSLTokenTypes.None)
            {
                tokenTags.SetClassifierTag(matchType);
            }

            return tokenTags;
        }

        public GLSLTokenTypes MatchTokenType(string token, Scope scope)
        {
            if (_tokens.Contains(token))
            {
                return GLSLTokenTypes.BuiltInVariable;
            }
            else
            {
                var variableInfo = _variableInfos.GetVariable(token, scope);
                if (variableInfo != null)
                {
                    return variableInfo.GLSLType;
                }
            }

            return GLSLTokenTypes.None;
        }

        public void Clear()
        {
            _inputVariables.Clear();
            _outputVariables.Clear();
            _uniformVariables.Clear();
            _localVariables.Clear();

            _variableInfos.Clear();
        }
    }
}

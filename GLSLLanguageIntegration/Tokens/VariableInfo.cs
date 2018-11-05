using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using System;

namespace GLSLLanguageIntegration.Tokens
{
    public class VariableInfo : TokenInfo
    {
        public SnapshotSpan DefinitionSpan { get; private set; }
        public Scope Scope { get; private set; }
        public string VariableType { get; private set; }

        public VariableInfo(SnapshotSpan span, Scope scope, string variableType, GLSLTokenTypes glslType) : base(span.GetText(), glslType)
        {
            DefinitionSpan = span;
            Scope = scope;
            VariableType = variableType;
        }

        public void Translate(ITextSnapshot textSnapshot)
        {
            DefinitionSpan = DefinitionSpan.Translated(textSnapshot);
        }

        protected override string GetTitle()
        {
            if (!GLSLType.IsVariable()) throw new NotImplementedException("Could not handle type " + Enum.GetName(typeof(GLSLTokenTypes), GLSLType));
            return "(" + GLSLType.GetDisplayName() + ") " + VariableType + " " + Token;
        }
    }
}

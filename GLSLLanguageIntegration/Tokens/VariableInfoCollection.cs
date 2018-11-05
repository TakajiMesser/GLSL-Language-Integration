using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Tokens
{
    public class VariableInfoCollection
    {
        private Dictionary<Scope, List<VariableInfo>> _variablesByScope = new Dictionary<Scope, List<VariableInfo>>();

        public void Add(VariableInfo variableInfo)
        {
            if (!_variablesByScope.ContainsKey(variableInfo.Scope))
            {
                _variablesByScope.Add(variableInfo.Scope, new List<VariableInfo>());
            }

            _variablesByScope[variableInfo.Scope].Add(variableInfo);
        }

        public void Translate(ITextSnapshot textSnapshot)
        {
            foreach (var variableInfo in _variablesByScope.Values.SelectMany(v => v))
            {
                variableInfo.Translate(textSnapshot);
            }
        }

        public void Remove(Span span)
        {
            foreach (var variables in _variablesByScope.Values)
            {
                for (var i = variables.Count - 1; i >= 0; i--)
                {
                    if (variables[i].DefinitionSpan.OverlapsWith(span))
                    {
                        variables.RemoveAt(i);
                    }
                }
            }
        }

        public void Clear() => _variablesByScope.Clear();

        public VariableInfo GetVariable(string token, Scope scope)
        {
            Scope currentScope = scope;

            do
            {
                if (_variablesByScope.ContainsKey(currentScope))
                {
                    var variable = _variablesByScope[currentScope].FirstOrDefault(v => v.Token == token);
                    
                    if (variable != null)
                    {
                        return variable;
                    }
                }

                currentScope = currentScope.Parent;
            }
            while (currentScope != null);

            return null;
        }

        public IEnumerable<VariableInfo> GetVariables(Scope scope)
        {
            Scope currentScope = scope;

            do
            {
                if (_variablesByScope.ContainsKey(currentScope))
                {
                    foreach (var variable in _variablesByScope[currentScope])
                    {
                        yield return variable;
                    }
                }

                currentScope = currentScope.Parent;
            }
            while (currentScope != null);
        }
    }
}

using GLSLLanguageIntegration.Tokens;

namespace GLSLLanguageIntegration.Utilities
{
    public static class ScopeExtensions
    {
        public static bool IsDescendentOf(this Scope parentScope, Scope scope)
        {
            foreach (var child in parentScope.Children)
            {
                if (child == scope || child.IsDescendentOf(scope))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsAncestorOf(this Scope childScope, Scope scope)
        {
            if (childScope.Parent == null)
            {
                return false;
            }
            else if (childScope.Parent == scope)
            {
                return true;
            }
            else
            {
                return childScope.Parent.IsAncestorOf(scope);
            }
        }
    }
}

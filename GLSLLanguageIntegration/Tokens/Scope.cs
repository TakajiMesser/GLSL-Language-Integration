using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using System.Collections.Generic;

namespace GLSLLanguageIntegration.Tokens
{
    public class Scope
    {
        public SnapshotSpan Span { get; private set; }

        public int Level { get; set; }
        public Scope Parent { get; set; }
        public List<Scope> Children { get; private set; } = new List<Scope>();

        public Scope(SnapshotSpan span)
        {
            Span = span;
        }

        public void AddChild(SnapshotSpan span)
        {
            var scope = GetScope(this, span);

            var childScope = new Scope(span)
            {
                Level = scope.Level + 1,
                Parent = scope
            };

            scope.Children.Add(childScope);
        }

        public Scope GetMatchingScope(SnapshotSpan span) => GetScope(this, span);

        public override int GetHashCode() => Span.GetHashCode();

        // SCOPE contains a level, a span, a parent, and children
        // Clearly, the scopes are forming a tree structure
        // THEREFORE, I need to store this tree!
        // We can just store the root scope.
        // When we check which scope we are in, we start at the root and see if our span is contained within the scope's span
        // If yes, we move to that child, and continue on until we are NOT contained within the scope's span. At that point we belong to the parent
        private Scope GetScope(Scope scope, SnapshotSpan span)
        {
            foreach (var child in scope.Children)
            {
                if (child.Span.Translated(span.Snapshot).Contains(span))
                {
                    return GetScope(child, span);
                }
            }

            return scope;
        }
    }
}

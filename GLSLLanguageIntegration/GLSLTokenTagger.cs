using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GLSLLanguageIntegration
{
    internal sealed class GLSLTokenTagger : ITagger<GLSLTokenTag>
    {
        private ITextBuffer _buffer;
        private Dictionary<string, GLSLTokenTypes> _tokenTypes;

        internal GLSLTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _tokenTypes = new Dictionary<string, GLSLTokenTypes>();
            //_tokenTypes[""] = GLSLTokenTypes.;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<GLSLTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var span in spans)
            {
                ITextSnapshotLine containingLine = span.Start.GetContainingLine();

                int currentLocation = containingLine.Start.Position;
                string[] tokens = containingLine.GetText().ToLower().Split(' ');

                foreach (var token in tokens)
                {
                    if (token.StartsWith("#"))
                    {
                        var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(currentLocation, token.Length));

                        if (tokenSpan.IntersectsWith(span))
                        {
                            yield return new TagSpan<GLSLTokenTag>(tokenSpan, new GLSLTokenTag(GLSLTokenTypes.Preprocessor));
                        }
                    }
                    else if (_tokenTypes.ContainsKey(token))
                    {
                        var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(currentLocation, token.Length));
                        
                        if (tokenSpan.IntersectsWith(span))
                        {
                            yield return new TagSpan<GLSLTokenTag>(tokenSpan, new GLSLTokenTag(_tokenTypes[token]));
                        }
                    }

                    // Add an extra character location because of the space
                    currentLocation += token.Length + 1;
                }
            }
        }
    }
}

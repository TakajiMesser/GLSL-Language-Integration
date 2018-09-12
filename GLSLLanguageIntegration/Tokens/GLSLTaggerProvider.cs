using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace GLSLLanguageIntegration.Tokens
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IGLSLTag))]
    [ContentType("glsl")]
    internal sealed class GLSLTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new GLSLTagger(buffer) as ITagger<T>);
        }
    }
}

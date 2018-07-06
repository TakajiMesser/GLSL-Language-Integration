using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace GLSLLanguageIntegration.Tokens
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IGLSLTag))]
    [ContentType("glsl")]
    internal sealed class GLSLTokenTagProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() =>
            {
                return new GLSLTokenTagger(buffer) as ITagger<T>;
            });
        }
    }
}

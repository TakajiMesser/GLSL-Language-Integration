using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace GLSLLanguageIntegration.Intellisense.Completions
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("glsl")]
    [Name("glslCompletion")]
    internal sealed class GLSLCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        internal IBufferTagAggregatorFactoryService _aggregatorFactory = null;

        public ICompletionSource TryCreateCompletionSource(ITextBuffer buffer)
        {
            var aggregator = _aggregatorFactory.CreateTagAggregator<IGLSLTag>(buffer);
            return new GLSLCompletionSource(buffer, aggregator);
        }
    }
}

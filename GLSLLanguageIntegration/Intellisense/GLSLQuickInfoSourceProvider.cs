using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace GLSLLanguageIntegration.Intellisense
{
    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [ContentType("glsl")]
    [Name("glslQuickInfo")]
    internal sealed class GLSLQuickInfoSourceProvider : IAsyncQuickInfoSourceProvider
    {
        [Import]
        private IBufferTagAggregatorFactoryService _aggregatorFactory = null;

        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() =>
            {
                var aggregator = _aggregatorFactory.CreateTagAggregator<IGLSLTag>(textBuffer);
                return new GLSLQuickInfoSource(textBuffer, aggregator);
            });
        }
    }
}

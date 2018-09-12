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
        private IBufferTagAggregatorFactoryService AggregatorFactory { get; set; }

        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() =>
            {
                var aggregator = AggregatorFactory.CreateTagAggregator<IGLSLTag>(textBuffer);
                return new GLSLQuickInfoSource(textBuffer, aggregator);
            });
        }
    }
}

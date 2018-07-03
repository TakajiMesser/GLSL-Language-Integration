using GLSLLanguageIntegration.Tags;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLLanguageIntegration.Intellisense
{
    [Export(typeof(IQuickInfoSourceProvider))]
    [ContentType("glsl")]
    [Name("glslQuickInfo")]
    internal sealed class GLSLQuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        private IBufferTagAggregatorFactoryService _aggregatorFactory = null;

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer buffer)
        {
            //return new GLSLQuickInfoSource(textBuffer, _aggregatorService.CreateTagAggregator<GLSLTokenTag>(textBuffer));
            return buffer.Properties.GetOrCreateSingletonProperty(() =>
            {
                var aggregator = _aggregatorFactory.CreateTagAggregator<IGLSLTag>(buffer);
                return new GLSLQuickInfoSource(buffer, aggregator);
            });
        }
    }
}

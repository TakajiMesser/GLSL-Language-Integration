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
        private IBufferTagAggregatorFactoryService _aggregatorService = null;

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new GLSLQuickInfoSource(textBuffer, _aggregatorService.CreateTagAggregator<GLSLTokenTag>(textBuffer));
        }
    }
}

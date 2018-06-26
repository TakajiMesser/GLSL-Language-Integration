using GLSLLanguageIntegration.Tags;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace GLSLLanguageIntegration.Classification
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("glsl")]
    [TagType(typeof(ClassificationTag))]
    internal sealed class GLSLClassifierProvider : ITaggerProvider
    {
        [Export]
        [Name("glsl")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition GLSLContentType = null;

        [Export]
        [FileExtension(".glsl")]
        [ContentType("glsl")]
        internal static FileExtensionToContentTypeDefinition GLSLFileType = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var aggregator = aggregatorFactory.CreateTagAggregator<GLSLTokenTag>(buffer);
            return new GLSLClassifier(buffer, aggregator, ClassificationTypeRegistry) as ITagger<T>;
        }
    }
}

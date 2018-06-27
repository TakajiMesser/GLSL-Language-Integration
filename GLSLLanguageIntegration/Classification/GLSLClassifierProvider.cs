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

        [Export]
        [FileExtension(".vert")]
        [ContentType("glsl")]
        internal static FileExtensionToContentTypeDefinition VertFileType = null;

        [Export]
        [FileExtension(".tesc")]
        [ContentType("glsl")]
        internal static FileExtensionToContentTypeDefinition TescFileType = null;

        [Export]
        [FileExtension(".tese")]
        [ContentType("glsl")]
        internal static FileExtensionToContentTypeDefinition TeseFileType = null;

        [Export]
        [FileExtension(".geom")]
        [ContentType("glsl")]
        internal static FileExtensionToContentTypeDefinition GeomFileType = null;

        [Export]
        [FileExtension(".frag")]
        [ContentType("glsl")]
        internal static FileExtensionToContentTypeDefinition FragFileType = null;

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

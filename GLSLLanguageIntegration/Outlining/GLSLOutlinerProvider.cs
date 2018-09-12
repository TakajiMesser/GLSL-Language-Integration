using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace GLSLLanguageIntegration.Outlining
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("glsl")]
    internal sealed class GLSLOutlinerProvider : ITaggerProvider
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
        internal IBufferTagAggregatorFactoryService AggregatorFactory { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() =>
            {
                var aggregator = AggregatorFactory.CreateTagAggregator<IGLSLTag>(buffer);
                return new GLSLOutliner(buffer, aggregator) as ITagger<T>;
            });
        }
    }
}

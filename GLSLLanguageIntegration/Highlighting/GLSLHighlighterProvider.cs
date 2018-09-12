using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace GLSLLanguageIntegration.Outlining
{
    [Export(typeof(IViewTaggerProvider))]
    [TagType(typeof(TextMarkerTag))]
    [ContentType("glsl")]
    internal sealed class GLSLHighlighterProvider : IViewTaggerProvider
    {
        [Export]
        [Name("glsl")]
        [BaseDefinition("text")]
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
        internal ITextSearchService TextSearchService { get; set; }

        [Import]
        internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }

        [Import]
        internal IBufferTagAggregatorFactoryService AggregatorFactory { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView.TextBuffer != buffer)
            {
                return null;
            }
            else
            {
                return buffer.Properties.GetOrCreateSingletonProperty(() =>
                {
                    var aggregator = AggregatorFactory.CreateTagAggregator<IGLSLTag>(buffer);
                    var navigator = TextStructureNavigatorSelector.GetTextStructureNavigator(buffer);
                    return new GLSLHighlighter(textView, buffer, TextSearchService, navigator, aggregator) as ITagger<T>;
                });
            }
        }
    }
}

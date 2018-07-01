using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GLSLLanguageIntegration.Tags
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IGLSLTag))]
    [ContentType("glsl")]
    internal sealed class GLSLTokenTagProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            //return new GLSLTokenTagger(buffer) as ITagger<T>;
            return buffer.Properties.GetOrCreateSingletonProperty(() =>
            {
                return new GLSLTokenTagger(buffer) as ITagger<T>;
            });
        }
    }
}

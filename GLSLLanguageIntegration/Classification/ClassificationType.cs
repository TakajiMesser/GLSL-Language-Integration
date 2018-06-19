using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GLSLLanguageIntegration.Classification
{
    /*Preprocessor,
    Comment,
    Keyword,
    Identifier,
    IntegerConstant,
    FloatingConstant,
    Operator,
    Semicolon,
    Bracket*/

    internal static class OrdinaryClassificationDefinition
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("preprocessor")]
        internal static ClassificationTypeDefinition glslPreprocessor = null;
    }
}

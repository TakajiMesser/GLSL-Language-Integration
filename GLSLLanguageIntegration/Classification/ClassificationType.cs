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
    /*IntegerConstant,
    FloatingConstant,
    Operator,
    Semicolon*/

    internal static class OrdinaryClassificationDefinition
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("preprocessor")]
        internal static ClassificationTypeDefinition glslPreprocessor = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("comment")]
        internal static ClassificationTypeDefinition glslComment = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("keyword")]
        internal static ClassificationTypeDefinition glslKeyword = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("type")]
        internal static ClassificationTypeDefinition glslType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("identifier")]
        internal static ClassificationTypeDefinition glslIdentifier = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("bracket")]
        internal static ClassificationTypeDefinition glslBracket = null;
    }
}

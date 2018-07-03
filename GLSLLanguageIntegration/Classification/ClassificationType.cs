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
        [Name(nameof(GLSLPreprocessor))]
        internal static ClassificationTypeDefinition glslPreprocessor = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLComment))]
        internal static ClassificationTypeDefinition glslComment = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLKeyword))]
        internal static ClassificationTypeDefinition glslKeyword = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLType))]
        internal static ClassificationTypeDefinition glslType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLIdentifier))]
        internal static ClassificationTypeDefinition glslIdentifier = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLBracket))]
        internal static ClassificationTypeDefinition glslBracket = null;
    }
}

using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace GLSLLanguageIntegration.Classification
{
    internal static class OrdinaryClassificationDefinition
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLDirective))]
        internal static ClassificationTypeDefinition glslDirective = null;

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
        [Name(nameof(GLSLInputVariable))]
        internal static ClassificationTypeDefinition glslInputVariable = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLOutputVariable))]
        internal static ClassificationTypeDefinition glslOutputVariable = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLUniformVariable))]
        internal static ClassificationTypeDefinition glslUniformVariable = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLBufferVariable))]
        internal static ClassificationTypeDefinition glslBufferVariable = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLSharedVariable))]
        internal static ClassificationTypeDefinition glslSharedVariable = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLBuiltInVariable))]
        internal static ClassificationTypeDefinition glslBuiltInVariable = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLLocalVariable))]
        internal static ClassificationTypeDefinition glslLocalVariable = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLParameterVariable))]
        internal static ClassificationTypeDefinition glslParameterVariable = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLStruct))]
        internal static ClassificationTypeDefinition glslStruct = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLIntegerConstant))]
        internal static ClassificationTypeDefinition glslIntegerConstant = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLFloatingConstant))]
        internal static ClassificationTypeDefinition glslFloatingConstant = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLBuiltInConstant))]
        internal static ClassificationTypeDefinition glslBuiltInConstant = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLFunction))]
        internal static ClassificationTypeDefinition glslFunction = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLBuiltInFunction))]
        internal static ClassificationTypeDefinition glslBuiltInFunction = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLOperator))]
        internal static ClassificationTypeDefinition glslOperator = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLSemicolon))]
        internal static ClassificationTypeDefinition glslSemicolon = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLParenthesis))]
        internal static ClassificationTypeDefinition glslParenthesis = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLCurlyBracket))]
        internal static ClassificationTypeDefinition glslCurlyBracket = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(nameof(GLSLSquareBracket))]
        internal static ClassificationTypeDefinition glslSquareBracket = null;
    }
}

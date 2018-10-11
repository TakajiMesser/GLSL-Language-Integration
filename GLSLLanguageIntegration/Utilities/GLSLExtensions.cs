using GLSLLanguageIntegration.Tokens;
using System;

namespace GLSLLanguageIntegration.Utilities
{
    public static class GLSLExtensions
    {
        public static string GetDisplayName(this GLSLTokenTypes type)
        {
            switch (type)
            {
                case GLSLTokenTypes.Directive:
                    return "Directive";
                case GLSLTokenTypes.Comment:
                    return "Comment";
                case GLSLTokenTypes.Keyword:
                    return "Keyword";
                case GLSLTokenTypes.Type:
                    return "Type";
                case GLSLTokenTypes.InputVariable:
                    return "Input Variable";
                case GLSLTokenTypes.OutputVariable:
                    return "Output Variable";
                case GLSLTokenTypes.UniformVariable:
                    return "Uniform Variable";
                case GLSLTokenTypes.BufferVariable:
                    return "Buffer Variable";
                case GLSLTokenTypes.SharedVariable:
                    return "Shared Variable";
                case GLSLTokenTypes.BuiltInVariable:
                    return "Built-In Variable";
                case GLSLTokenTypes.LocalVariable:
                    return "Local Variable";
                case GLSLTokenTypes.ParameterVariable:
                    return "Parameter Variable";
                case GLSLTokenTypes.Struct:
                    return "Struct";
                case GLSLTokenTypes.IntegerConstant:
                    return "Integer Constant";
                case GLSLTokenTypes.FloatingConstant:
                    return "Floating Constant";
                case GLSLTokenTypes.BuiltInConstant:
                    return "Built-In Constant";
                case GLSLTokenTypes.Function:
                    return "Function";
                case GLSLTokenTypes.BuiltInFunction:
                    return "Built-In Function";
                case GLSLTokenTypes.Operator:
                    return "Operator";
                case GLSLTokenTypes.Semicolon:
                    return "Semicolon";
                case GLSLTokenTypes.Parenthesis:
                    return "Parenthesis";
                case GLSLTokenTypes.CurlyBracket:
                    return "Curly Bracket";
                case GLSLTokenTypes.SquareBracket:
                    return "Square Bracket";
                default:
                    throw new NotImplementedException("Could not handle type " + type);
            }
        }

        public static bool IsVariable(this GLSLTokenTypes type) =>
            type == GLSLTokenTypes.InputVariable
            || type == GLSLTokenTypes.OutputVariable
            || type == GLSLTokenTypes.UniformVariable
            || type == GLSLTokenTypes.BufferVariable
            || type == GLSLTokenTypes.SharedVariable
            || type == GLSLTokenTypes.BuiltInVariable
            || type == GLSLTokenTypes.LocalVariable
            || type == GLSLTokenTypes.ParameterVariable;

        public static bool IsConstant(this GLSLTokenTypes type) =>
            type == GLSLTokenTypes.IntegerConstant
            || type == GLSLTokenTypes.FloatingConstant
            || type == GLSLTokenTypes.BuiltInConstant;

        public static bool IsFunction(this GLSLTokenTypes type) =>
            type == GLSLTokenTypes.Function
            || type == GLSLTokenTypes.BuiltInFunction;

        public static bool IsBuiltIn(this GLSLTokenTypes type) =>
            type == GLSLTokenTypes.BuiltInConstant
            || type == GLSLTokenTypes.BuiltInFunction
            || type == GLSLTokenTypes.BuiltInVariable;

        public static bool IsPreprocessor(this GLSLTokenTypes type) =>
            type == GLSLTokenTypes.Directive
            || type == GLSLTokenTypes.Comment;
    }
}

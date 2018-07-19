using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace GLSLLanguageIntegration.Classification
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLPreprocessor))]
    [Name(nameof(GLSLPreprocessor))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLPreprocessor : ClassificationFormatDefinition
    {
        public GLSLPreprocessor()
        {
            DisplayName = nameof(GLSLPreprocessor);
            ForegroundColor = Colors.BlueViolet;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLComment))]
    [Name(nameof(GLSLComment))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLComment : ClassificationFormatDefinition
    {
        public GLSLComment()
        {
            DisplayName = nameof(GLSLComment);
            ForegroundColor = Colors.Green;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLKeyword))]
    [Name(nameof(GLSLKeyword))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLKeyword : ClassificationFormatDefinition
    {
        public GLSLKeyword()
        {
            DisplayName = nameof(GLSLKeyword);
            ForegroundColor = Colors.MediumBlue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLType))]
    [Name(nameof(GLSLType))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLType : ClassificationFormatDefinition
    {
        public GLSLType()
        {
            DisplayName = nameof(GLSLType);
            ForegroundColor = Colors.LightGreen;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLInputVariable))]
    [Name(nameof(GLSLInputVariable))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLInputVariable : ClassificationFormatDefinition
    {
        public GLSLInputVariable()
        {
            DisplayName = nameof(GLSLInputVariable);
            ForegroundColor = Colors.LightPink;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLOutputVariable))]
    [Name(nameof(GLSLOutputVariable))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLOutputVariable : ClassificationFormatDefinition
    {
        public GLSLOutputVariable()
        {
            DisplayName = nameof(GLSLOutputVariable);
            ForegroundColor = Colors.Pink;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLUniformVariable))]
    [Name(nameof(GLSLUniformVariable))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLUniformVariable : ClassificationFormatDefinition
    {
        public GLSLUniformVariable()
        {
            DisplayName = nameof(GLSLUniformVariable);
            ForegroundColor = Colors.DeepPink;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLBufferVariable))]
    [Name(nameof(GLSLBufferVariable))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLBufferVariable : ClassificationFormatDefinition
    {
        public GLSLBufferVariable()
        {
            DisplayName = nameof(GLSLBufferVariable);
            ForegroundColor = Colors.MediumVioletRed;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLSharedVariable))]
    [Name(nameof(GLSLSharedVariable))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLSharedVariable : ClassificationFormatDefinition
    {
        public GLSLSharedVariable()
        {
            DisplayName = nameof(GLSLSharedVariable);
            ForegroundColor = Colors.PaleVioletRed;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLBuiltInVariable))]
    [Name(nameof(GLSLBuiltInVariable))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLBuiltInVariable : ClassificationFormatDefinition
    {
        public GLSLBuiltInVariable()
        {
            DisplayName = nameof(GLSLBuiltInVariable);
            ForegroundColor = Colors.YellowGreen;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLLocalVariable))]
    [Name(nameof(GLSLLocalVariable))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLLocalVariable : ClassificationFormatDefinition
    {
        public GLSLLocalVariable()
        {
            DisplayName = nameof(GLSLLocalVariable);
            ForegroundColor = Colors.WhiteSmoke;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLStruct))]
    [Name(nameof(GLSLStruct))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLStruct : ClassificationFormatDefinition
    {
        public GLSLStruct()
        {
            DisplayName = nameof(GLSLStruct);
            ForegroundColor = Colors.ForestGreen;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLIntegerConstant))]
    [Name(nameof(GLSLIntegerConstant))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLIntegerConstant : ClassificationFormatDefinition
    {
        public GLSLIntegerConstant()
        {
            DisplayName = nameof(GLSLIntegerConstant);
            ForegroundColor = Colors.LightSkyBlue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLFloatingConstant))]
    [Name(nameof(GLSLFloatingConstant))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLFloatingConstant : ClassificationFormatDefinition
    {
        public GLSLFloatingConstant()
        {
            DisplayName = nameof(GLSLFloatingConstant);
            ForegroundColor = Colors.LightSteelBlue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLBuiltInConstant))]
    [Name(nameof(GLSLBuiltInConstant))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLBuiltInConstant : ClassificationFormatDefinition
    {
        public GLSLBuiltInConstant()
        {
            DisplayName = nameof(GLSLBuiltInConstant);
            ForegroundColor = Colors.PowderBlue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLFunction))]
    [Name(nameof(GLSLFunction))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLFunction : ClassificationFormatDefinition
    {
        public GLSLFunction()
        {
            DisplayName = nameof(GLSLFunction);
            ForegroundColor = Colors.IndianRed;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLBuiltInFunction))]
    [Name(nameof(GLSLBuiltInFunction))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLBuiltInFunction : ClassificationFormatDefinition
    {
        public GLSLBuiltInFunction()
        {
            DisplayName = nameof(GLSLBuiltInFunction);
            ForegroundColor = Colors.OrangeRed;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLOperator))]
    [Name(nameof(GLSLOperator))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLOperator : ClassificationFormatDefinition
    {
        public GLSLOperator()
        {
            DisplayName = nameof(GLSLOperator);
            ForegroundColor = Colors.SlateGray;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLSemicolon))]
    [Name(nameof(GLSLSemicolon))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLSemicolon : ClassificationFormatDefinition
    {
        public GLSLSemicolon()
        {
            DisplayName = nameof(GLSLSemicolon);
            ForegroundColor = Colors.DimGray;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLParenthesis))]
    [Name(nameof(GLSLParenthesis))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLParenthesis : ClassificationFormatDefinition
    {
        public GLSLParenthesis()
        {
            DisplayName = nameof(GLSLParenthesis);
            ForegroundColor = Colors.Gray;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLCurlyBracket))]
    [Name(nameof(GLSLCurlyBracket))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLCurlyBracket : ClassificationFormatDefinition
    {
        public GLSLCurlyBracket()
        {
            DisplayName = nameof(GLSLCurlyBracket);
            ForegroundColor = Colors.Gray;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLSquareBracket))]
    [Name(nameof(GLSLSquareBracket))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLSquareBracket : ClassificationFormatDefinition
    {
        public GLSLSquareBracket()
        {
            DisplayName = nameof(GLSLSquareBracket);
            ForegroundColor = Colors.Gray;
        }
    }
}

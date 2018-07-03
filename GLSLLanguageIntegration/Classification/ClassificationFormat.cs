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
    [ClassificationType(ClassificationTypeNames = nameof(GLSLIdentifier))]
    [Name(nameof(GLSLIdentifier))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLIdentifier : ClassificationFormatDefinition
    {
        public GLSLIdentifier()
        {
            DisplayName = nameof(GLSLIdentifier);
            ForegroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = nameof(GLSLBracket))]
    [Name(nameof(GLSLBracket))]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class GLSLBracket : ClassificationFormatDefinition
    {
        public GLSLBracket()
        {
            DisplayName = nameof(GLSLBracket);
            ForegroundColor = Colors.Gray;
        }
    }

    /*IntegerConstant,
    FloatingConstant,
    Operator,
    Semicolon*/
}

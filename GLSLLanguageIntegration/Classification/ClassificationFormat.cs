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
    [ClassificationType(ClassificationTypeNames = "preprocessor")]
    [Name("preprocessor")]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class GLSLPreprocessor : ClassificationFormatDefinition
    {
        public GLSLPreprocessor()
        {
            DisplayName = "preprocessor";
            ForegroundColor = Colors.BlueViolet;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "comment")]
    [Name("comment")]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class GLSLComment : ClassificationFormatDefinition
    {
        public GLSLComment()
        {
            DisplayName = "comment";
            ForegroundColor = Colors.Green;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "keyword")]
    [Name("keyword")]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class GLSLKeyword : ClassificationFormatDefinition
    {
        public GLSLKeyword()
        {
            DisplayName = "keyword";
            ForegroundColor = Colors.MediumBlue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "type")]
    [Name("type")]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class GLSLType : ClassificationFormatDefinition
    {
        public GLSLType()
        {
            DisplayName = "type";
            ForegroundColor = Colors.LightGreen;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "identifier")]
    [Name("identifier")]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class GLSLIdentifier : ClassificationFormatDefinition
    {
        public GLSLIdentifier()
        {
            DisplayName = "identifier";
            ForegroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "bracket")]
    [Name("bracket")]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class GLSLBracket : ClassificationFormatDefinition
    {
        public GLSLBracket()
        {
            DisplayName = "bracket";
            ForegroundColor = Colors.Gray;
        }
    }

    /*IntegerConstant,
    FloatingConstant,
    Operator,
    Semicolon*/
}

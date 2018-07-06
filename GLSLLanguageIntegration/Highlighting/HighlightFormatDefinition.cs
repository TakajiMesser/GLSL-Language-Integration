using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace GLSLLanguageIntegration.Outlining
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("MarkerFormatDefinition/HighlightWordFormatDefinition")]
    [UserVisible(true)]
    internal class HighlightWordFormatDefinition : MarkerFormatDefinition
    {
        public HighlightWordFormatDefinition()
        {
            BackgroundColor = Colors.LightBlue;
            //ForegroundColor = Colors.DarkBlue;
            DisplayName = "Highlight Word";
            ZOrder = 5;
        }
    }
}

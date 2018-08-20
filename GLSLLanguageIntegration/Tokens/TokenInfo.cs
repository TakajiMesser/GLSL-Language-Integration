using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Utilities;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GLSLLanguageIntegration.Tokens
{
    public class TokenInfo
    {
        public string Token { get; private set; }
        public GLSLTokenTypes GLSLType { get; private set; }
        public string Definition { get; set; }

        public TokenInfo(string token, GLSLTokenTypes glslType)
        {
            Token = token;
            GLSLType = glslType;
        }

        public object ToQuickInfo()
        {
            var stackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical
            };

            var dockPanel = new DockPanel();

            var image = new Image()
            {
                Source = GetImageSource(),
                Margin = new System.Windows.Thickness(0, 0, 6, 0)
            };
            DockPanel.SetDock(image, Dock.Left);

            var titleBlock = new TextBlock()
            {
                Foreground = Brushes.White,
                Text = GetTitle()
            };
            DockPanel.SetDock(titleBlock, Dock.Right);

            dockPanel.Children.Add(image);
            dockPanel.Children.Add(titleBlock);

            stackPanel.Children.Add(dockPanel);
            if (!string.IsNullOrEmpty(Definition))
            {
                var definitionBlock = new TextBlock()
                {
                    Text = Definition,
                    Foreground = Brushes.White
                };

                stackPanel.Children.Add(definitionBlock);
            }

            return stackPanel;
        }

        protected virtual string GetTitle()
        {
            switch (GLSLType)
            {
                case GLSLTokenTypes.Preprocessor:
                    return "Preprocessor";
                default:
                    return "(" + GLSLType.GetDisplayName() + ") " + Token;
            }
        }

        public BitmapImage GetImageSource()
        {
            switch (GLSLType)
            {
                case GLSLTokenTypes.Preprocessor:
                    return Resources.data_number_on_16x.ToBitmapImage();
                case GLSLTokenTypes.Keyword:
                    return Resources.Constant_16x.ToBitmapImage();
                case GLSLTokenTypes.Type:
                    return Resources.Numeric_16x.ToBitmapImage();
                default:
                    return Resources.Operator_left_16x.ToBitmapImage();
            }
        }
    }
}

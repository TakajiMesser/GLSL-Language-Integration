using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Utilities;
using System;
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

            var titleBlock = GetTitle();
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

        private TextBlock GetTitle()
        {
            var textBlock = new TextBlock()
            {
                Foreground = Brushes.White
            };

            switch (GLSLType)
            {
                case GLSLTokenTypes.Preprocessor:
                    textBlock.Text = "Preprocessor";
                    break;
                case GLSLTokenTypes.Keyword:
                    textBlock.Text = "(Keyword) " + Token;
                    break;
                case GLSLTokenTypes.Type:
                    textBlock.Text = "(Type) " + Token;
                    break;
                case GLSLTokenTypes.InputVariable:
                    textBlock.Text = "(Input Variable) " + Token;
                    break;
                case GLSLTokenTypes.OutputVariable:
                    textBlock.Text = "(Output Variable) " + Token;
                    break;
                case GLSLTokenTypes.UniformVariable:
                    textBlock.Text = "(Uniform Variable) " + Token;
                    break;
                default:
                    throw new NotImplementedException("Could not handle type " + Enum.GetName(typeof(GLSLTokenTypes), GLSLType));
            }

            return textBlock;
        }

        private BitmapImage GetImageSource()
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

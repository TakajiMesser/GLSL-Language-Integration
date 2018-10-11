using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Properties;
using GLSLLanguageIntegration.Spans;
using GLSLLanguageIntegration.Tokens;
using GLSLLanguageIntegration.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Text.RegularExpressions;

namespace GLSLLanguageIntegration.Taggers
{
    public class GLSLConstantTagger : IGLSLTagger
    {
        public const GLSLTokenTypes INT_TOKEN_TYPE = GLSLTokenTypes.IntegerConstant;
        public const GLSLTokenTypes FLOAT_TOKEN_TYPE = GLSLTokenTypes.FloatingConstant;
        public const GLSLTokenTypes BUILT_IN_TOKEN_TYPE = GLSLTokenTypes.BuiltInConstant;

        private TokenSet _builtInTokens = new TokenSet(Resources.Constants, BUILT_IN_TOKEN_TYPE);

        public object GetQuickInfo(string token) => _builtInTokens.Contains(token) ? _builtInTokens.GetInfo(token).ToQuickInfo() : null;

        public TokenTagCollection Match(SnapshotSpan span)
        {
            var tokenTags = new TokenTagCollection(span);

            string token = span.GetText();
            int position = span.Start + token.Length;

            if (_builtInTokens.Contains(token))
            {
                tokenTags.SetClassifierTag(BUILT_IN_TOKEN_TYPE);
            }
            else if (IsFloatingConstant(token))
            {
                tokenTags.SetClassifierTag(FLOAT_TOKEN_TYPE);
            }
            else if (IsIntegerConstant(token))
            {
                var isFloat = false;

                // Now that we have matched with an integer, look ahead to see if we are followed by a decimal
                if (span.Snapshot.GetText(span.End, 1) == ".")
                {
                    var nConsumed = 1;
                    var text = span.Snapshot.GetText();

                    for (var i = span.End.Position + 1; i < text.Length; i++)
                    {
                        if (char.IsNumber(text[i]))
                        {
                            isFloat = true;
                            nConsumed++;
                        }
                        else if (text[i] == 'f' || text[i] == 'F')
                        {
                            nConsumed++;
                            break;
                        }
                        else if (text[i] == 'l')
                        {
                            if (text[i + 1] == 'f' || text[i + 1] == 'F')
                            {
                                nConsumed += 2;
                            }
                            else
                            {
                                isFloat = false;
                            }
                            
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (isFloat)
                    {
                        tokenTags.ClassifierTagSpan = new TagSpan<GLSLClassifierTag>(span.Extended(nConsumed), new GLSLClassifierTag(FLOAT_TOKEN_TYPE));
                    }
                }

                if (!isFloat)
                {
                    tokenTags.SetClassifierTag(INT_TOKEN_TYPE);
                }
            }

            return tokenTags;
        }

        private bool IsFloatingConstant(string token)
        {
            // By this point we have already split the token on the '.' character, so this will never match a decimal value...
            //return Regex.Match(token, @"^\d+\.\d+(f|F|lf|LF)?$").Success;

            // Instead we must first match with an integer, then attempt to consume more characters after that
            // This handles the case where there is no decimal point, but it is followed by the appropriate character for floats
            return Regex.Match(token, @"^\d+(f|F|lf|LF)$").Success;
        }

        private bool IsIntegerConstant(string token) => Regex.Match(token, @"^(0x|0X)*\d+(u|U)*$").Success;

        public void Clear() { }
    }
}

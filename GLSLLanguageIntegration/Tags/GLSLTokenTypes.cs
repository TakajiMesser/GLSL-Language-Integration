using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLLanguageIntegration.Tags
{
    public enum GLSLTokenTypes
    {
        Preprocessor,
        Comment,
        Keyword,
        Types,
        Identifier,
        IntegerConstant,
        FloatingConstant,
        Operator,
        Semicolon,
        Bracket
    }
}

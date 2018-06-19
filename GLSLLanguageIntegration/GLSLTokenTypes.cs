using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLLanguageIntegration
{
    public enum GLSLTokenTypes
    {
        Preprocessor,
        Comment,
        Keyword,
        Identifier,
        IntegerConstant,
        FloatingConstant,
        Operator,
        Semicolon,
        Bracket
    }
}

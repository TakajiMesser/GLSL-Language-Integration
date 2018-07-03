using GLSLLanguageIntegration.Spans;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLLanguageIntegration.Taggers
{
    public interface IGLSLTagger
    {
        GLSLSpanResult Match(string token, int position, SnapshotSpan span);
    }
}

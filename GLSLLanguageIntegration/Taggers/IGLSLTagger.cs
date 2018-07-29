using GLSLLanguageIntegration.Spans;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public interface IGLSLTagger
    {
        GLSLSpanResult Match(SnapshotSpan span);
        void Clear();
    }
}

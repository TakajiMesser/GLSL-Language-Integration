using GLSLLanguageIntegration.Spans;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public interface IGLSLTagger
    {
        GLSLSpanResult Match(string token, int position, SnapshotSpan span);
        void Clear();
    }
}

using GLSLLanguageIntegration.Spans;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public interface IGLSLTagger
    {
        SpanResult Match(SnapshotSpan span);
        void Clear();
    }
}

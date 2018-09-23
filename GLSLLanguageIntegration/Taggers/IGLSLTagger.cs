using GLSLLanguageIntegration.Spans;
using Microsoft.VisualStudio.Text;

namespace GLSLLanguageIntegration.Taggers
{
    public interface IGLSLTagger
    {
        TokenTagCollection Match(SnapshotSpan span);
        void Clear();
    }
}

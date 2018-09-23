using GLSLLanguageIntegration.Classification;
using GLSLLanguageIntegration.Outlining;
using GLSLLanguageIntegration.Tokens;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Spans
{
    public class TokenTagCollection
    {
        public SnapshotSpan Span { get; private set; }
        public List<TagSpan<GLSLOutlineTag>> OutlineTagSpans { get; private set; } = new List<TagSpan<GLSLOutlineTag>>();
        public int Consumed { get; set; }

        public TagSpan<GLSLClassifierTag> ClassifierTagSpan
        {
            get => _classifierTagSpan;
            set
            {
                var translatedSpan = value.Span.TranslateTo(Span.Snapshot, SpanTrackingMode.EdgeExclusive);

                if (Span.OverlapsWith(translatedSpan))
                {
                    _classifierTagSpan = new TagSpan<GLSLClassifierTag>(translatedSpan, value.Tag);
                }
            }
        }

        public IEnumerable<TagSpan<IGLSLTag>> TagSpans
        {
            get
            {
                if (ClassifierTagSpan != null)
                {
                    yield return new TagSpan<IGLSLTag>(ClassifierTagSpan.Span, ClassifierTagSpan.Tag);
                }

                foreach (var outlineTagSpan in OutlineTagSpans)
                {
                    yield return new TagSpan<IGLSLTag>(outlineTagSpan.Span, outlineTagSpan.Tag);
                }
            }
        }

        private TagSpan<GLSLClassifierTag> _classifierTagSpan;

        public TokenTagCollection(SnapshotSpan span) => Span = span;

        public void SetClassifierTag(GLSLTokenTypes tokenType)
        {
            ClassifierTagSpan = new TagSpan<GLSLClassifierTag>(Span, new GLSLClassifierTag(tokenType));
        }

        public void SetClassifierTag(IEnumerable<TagSpan<GLSLClassifierTag>> tagSpans)
        {
            foreach (var tagSpan in tagSpans)
            {
                var translatedSpan = tagSpan.Span.TranslateTo(Span.Snapshot, SpanTrackingMode.EdgeExclusive);

                if (Span.OverlapsWith(translatedSpan))
                {
                    ClassifierTagSpan = tagSpan;
                    break;
                }
            }
        }

        public void AddOutlineTagSpan(TagSpan<GLSLOutlineTag> tagSpan)
        {
            var translatedSpan = tagSpan.Span.TranslateTo(Span.Snapshot, SpanTrackingMode.EdgeExclusive);

            if (Span.OverlapsWith(translatedSpan))
            {
                OutlineTagSpans.Add(new TagSpan<GLSLOutlineTag>(translatedSpan, tagSpan.Tag));
            }
        }

        public void AddOutlineTagSpans(IEnumerable<TagSpan<GLSLOutlineTag>> tagSpans)
        {
            foreach (var tagSpan in tagSpans)
            {
                var translatedSpan = tagSpan.Span.TranslateTo(Span.Snapshot, SpanTrackingMode.EdgeExclusive);

                if (Span.OverlapsWith(translatedSpan))
                {
                    OutlineTagSpans.Add(new TagSpan<GLSLOutlineTag>(translatedSpan, tagSpan.Tag));
                }
            }
        }
    }
}

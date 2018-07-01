using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GLSLLanguageIntegration.Tags.Spans
{
    public class SpanBuilder
    {
        public int? Start { get; set; }
        public int? End { get; set; }

        public int Length => End.Value - Start.Value;

        public ITextSnapshot Snapshot { get; set; }

        public bool IsReady => Start.HasValue && End.HasValue && Snapshot != null;
        public bool IsPartial => Start.HasValue && !End.HasValue && Snapshot != null;
        public bool IsEmpty => !Start.HasValue && !End.HasValue;

        public void Clear()
        {
            Start = null;
            End = null;
        }

        public SnapshotSpan ToSpan()
        {
            return new SnapshotSpan(Snapshot, Start.Value, Length);
        }
    }
}

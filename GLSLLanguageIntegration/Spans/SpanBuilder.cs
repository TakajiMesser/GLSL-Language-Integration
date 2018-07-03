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

namespace GLSLLanguageIntegration.Spans
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

        /// <summary>
        /// Consume characters from the provided string until any one of the terminating strings are found.
        /// </summary>
        /// <returns>The earliest position of a terminator, or -1 if no terminators were found.</returns>
        public int ConsumeUntil(string text, int start, params string[] terminators)
        {
            for (var i = start; i < text.Length; i++)
            {
                char character = text[i];

                foreach (var terminator in terminators)
                {
                    if (character == terminator.Last() && i > start + terminator.Length - 1)
                    {
                        if (text.Substring(i - terminator.Length + 1, terminator.Length) == terminator)
                        {
                            return i;
                        }
                    }
                }
            }

            return -1;
        }

        public SnapshotSpan ToSpan() => new SnapshotSpan(Snapshot, Start.Value, Length);

        public SnapshotSpan ToSpan(int leftOffset, int rightOffset) => new SnapshotSpan(Snapshot, Start.Value + leftOffset, Length - leftOffset - rightOffset);
    }
}

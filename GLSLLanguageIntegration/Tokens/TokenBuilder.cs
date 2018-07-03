﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GLSLLanguageIntegration.Tags;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GLSLLanguageIntegration.Tokens
{
    public class TokenBuilder
    {
        public int StartPosition { get; private set; }
        public int EndPosition => StartPosition + _builder.Length;
        public int Length => _builder.Length;
        public ITextSnapshot Snapshot { get; set; }
        public SnapshotSpan Span => new SnapshotSpan(Snapshot, StartPosition, Length);

        private StringBuilder _builder = new StringBuilder();

        public TagSpan<GLSLTokenTag> GetTag(SnapshotSpan span, GLSLTokenTypes type)
        {
            var tokenSpan = Span;

            if (tokenSpan.IntersectsWith(span))
            {
                return new TagSpan<GLSLTokenTag>(tokenSpan, new GLSLTokenTag(type));
            }

            return null;
        }

        public void Append(char value, int position)
        {
            if (_builder.Length == 0)
            {
                StartPosition = position;
            }
            else if (position - StartPosition != _builder.Length)
            {
                throw new ArgumentOutOfRangeException("Position is incorrect (Do you need to call Clear() to flush the buffer first?)");
            }

            _builder.Append(value);
        }

        public void Clear()
        {
            _builder.Clear();
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}

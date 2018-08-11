using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GLSLLanguageIntegration.Intellisense.Signatures
{
    internal class GLSLSignatureHelpSource : ISignatureHelpSource
    {
        private ITextBuffer _textBuffer;

        public GLSLSignatureHelpSource(ITextBuffer textBuffer)
        {
            _textBuffer = textBuffer;
        }

        public void AugmentSignatureHelpSession(ISignatureHelpSession session, IList<ISignature> signatures)
        {
            var snapshot = _textBuffer.CurrentSnapshot;
            int position = session.GetTriggerPoint(_textBuffer).GetPosition(snapshot);

            var applicableToSpan = _textBuffer.CurrentSnapshot.CreateTrackingSpan(new Span(position, 0), SpanTrackingMode.EdgeInclusive, 0);

            // Add to signatures list using CreateSignature() method here
            signatures.Add(CreateSignature(_textBuffer, "add(int firstInt, int secondInt)", "Documentation", applicableToSpan));
        }

        public ISignature GetBestMatch(ISignatureHelpSession session)
        {
            if (session.Signatures.Count > 0)
            {
                var applicableToSpan = session.Signatures.First().ApplicableToSpan;
                var text = applicableToSpan.GetText(applicableToSpan.TextBuffer.CurrentSnapshot);

                if (text.Trim().Equals("add"))
                {
                    return session.Signatures.First();
                }
            }

            return null;
        }

        private GLSLSignature CreateSignature(ITextBuffer textBuffer, string methodSignature, string methodDocumentation, ITrackingSpan span)
        {
            var signature = new GLSLSignature(textBuffer, methodSignature, methodDocumentation, null);
            textBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(signature.OnSubjectBufferChanged);

            //find the parameters in the method signature (expect methodname(one, two)
            string[] parameters = methodSignature.Split(new char[] { '(', ',', ')' });
            List<IParameter> paramList = new List<IParameter>();

            int locusSearchStart = 0;
            for (int i = 1; i < parameters.Length; i++)
            {
                string param = parameters[i].Trim();

                if (!string.IsNullOrEmpty(param))
                {
                    // find where this parameter is located in the method signature
                    int locusStart = methodSignature.IndexOf(param, locusSearchStart);
                    if (locusStart >= 0)
                    {
                        Span locus = new Span(locusStart, param.Length);
                        locusSearchStart = locusStart + param.Length;
                        paramList.Add(new GLSLParameter(param, "Documentation for the parameter.", locus, signature));
                    }
                }
            }

            signature.Parameters = new ReadOnlyCollection<IParameter>(paramList);
            signature.ApplicableToSpan = span;
            signature.ComputeCurrentParameter();

            return signature;
        }

        private bool _isDisposed = false;
        public void Dispose()
        {
            if (!_isDisposed)
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }
    }
}

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GLSLLanguageIntegration.Intellisense.Signatures
{
    internal class GLSLSignature : ISignature
    {
        public string Content { get; internal set; }
        public string PrettyPrintedContent { get; internal set; }
        public string Documentation { get; internal set; }

        public ReadOnlyCollection<IParameter> Parameters { get; internal set; }
        public ITrackingSpan ApplicableToSpan { get; internal set; }

        public IParameter CurrentParameter
        {
            get => _currentParameter;
            set
            {
                if (_currentParameter != value)
                {
                    var previousParameter = _currentParameter;
                    _currentParameter = value;
                    CurrentParameterChanged?.Invoke(this, new CurrentParameterChangedEventArgs(previousParameter, _currentParameter));
                }
            }
        }

        private ITextBuffer _subjectBuffer;
        private IParameter _currentParameter;

        public event EventHandler<CurrentParameterChangedEventArgs> CurrentParameterChanged;

        internal GLSLSignature(ITextBuffer subjectBuffer, string content, string doc, ReadOnlyCollection<IParameter> parameters)
        {
            _subjectBuffer = subjectBuffer;
            _subjectBuffer.Changed += OnSubjectBufferChanged;

            Content = content;
            Documentation = doc;
            Parameters = parameters;
        }

        internal void OnSubjectBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            ComputeCurrentParameter();
        }

        internal void ComputeCurrentParameter()
        {
            if (Parameters.Count == 0)
            {
                _currentParameter = null;
            }
            else
            {
                int commaCount = GetCommaCount();

                CurrentParameter = (commaCount < Parameters.Count)
                    ? Parameters[commaCount]
                    : Parameters.Last();
            }
        }

        private int GetCommaCount()
        {
            var signatureText = ApplicableToSpan.GetText(_subjectBuffer.CurrentSnapshot);
            int commaCount = 0;

            for (var i = 0; i < signatureText.Length; i++)
            {
                if (signatureText[i] == ',')
                {
                    commaCount++;
                }
            }

            return commaCount;
        }
    }
}

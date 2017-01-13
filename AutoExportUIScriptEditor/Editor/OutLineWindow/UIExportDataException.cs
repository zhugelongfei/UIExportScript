using System;

namespace AutoExportScriptData
{
    public class UIExportDataException : Exception
    {
        private UIProgramData[] errorObjs;

        public UIProgramData[] ErrorObjs
        {
            get { return errorObjs; }
        }

        public UIExportDataException(string message)
            : base(message)
        {

        }

        public UIExportDataException(string message, params UIProgramData[] errorObjs)
            : this(message)
        {
            this.errorObjs = errorObjs;
        }
    }
}
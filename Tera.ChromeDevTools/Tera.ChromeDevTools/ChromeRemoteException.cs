using System;
using System.Linq;
using System.Runtime.Serialization;
using BaristaLabs.ChromeDevTools.Runtime.Runtime;

namespace Tera.ChromeDevTools
{
    [Serializable]
    public class ChromeRemoteException : Exception
    {
        private ExceptionDetails exceptionDetails;


        public ChromeRemoteException(ExceptionDetails exceptionDetails) : base("Remote Error:" + exceptionDetails.Text + " -> " + exceptionDetails.Exception.ClassName + " -> " + exceptionDetails.Exception.Description)
        {
            this.exceptionDetails = exceptionDetails;
        }

        public long Line
        {
            get { return exceptionDetails.LineNumber; }
        }
        public long Column
        {
            get { return exceptionDetails.ColumnNumber; }
        }

        public string RemoteStackTrace
        {
            get
            {
                return
                    exceptionDetails.StackTrace != null ?
                exceptionDetails.StackTrace.Description + Environment.NewLine +
                    string.Join("\n", exceptionDetails.StackTrace?.CallFrames.Select(f => f.FunctionName)) : "";
            }
        }
    }
}
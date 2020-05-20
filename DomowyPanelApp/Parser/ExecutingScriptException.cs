using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpit.Parser
{
    [global::System.Serializable]
    public class ExecutingScriptException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ExecutingScriptException() { }
        public ExecutingScriptException(string message) : base(message) { }
        public ExecutingScriptException(string message, Exception inner) : base(message, inner) { }
        protected ExecutingScriptException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

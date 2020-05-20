using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpit.Parser
{
    [global::System.Serializable]
    public class UserErrorException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public UserErrorException() { }
        public UserErrorException(string message) : base(message) { }
        public UserErrorException(string message, Exception inner) : base(message, inner) { }
        protected UserErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

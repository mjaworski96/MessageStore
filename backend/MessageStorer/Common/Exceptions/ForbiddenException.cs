using System;
using System.Runtime.Serialization;

namespace Common.Exceptions
{
    public class ForbiddenException : HttpException
    {
        public override int StatusCode => 403;
        public ForbiddenException(string message) : base(message)
        {
        }

        public ForbiddenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ForbiddenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

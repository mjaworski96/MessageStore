using System;
using System.Runtime.Serialization;

namespace Common.Exceptions
{
    public class ConflictException : HttpException
    {
        public override int StatusCode => 409;

        public ConflictException(string message) : base(message)
        {
        }

        public ConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

     }
}

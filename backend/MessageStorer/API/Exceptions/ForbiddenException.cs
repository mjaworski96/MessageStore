using API.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace API.Exceptions
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

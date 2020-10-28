using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace API.Exceptions
{
    public class BadRequestException : HttpException
    {
        public override int StatusCode => 400;

        public BadRequestException(string message) : base(message)
        {
        }

        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

      }
}

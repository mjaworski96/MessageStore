﻿using System;
using System.Runtime.Serialization;

namespace Common.Exceptions
{
    public class NotFoundException : HttpException
    {
        public override int StatusCode => 404;

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

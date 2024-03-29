﻿using System;
using System.Runtime.Serialization;

namespace MessengerIntegration.Exceptions
{
    public class ApiRequestException : Exception
    {
        public ApiRequestException()
        {
        }

        public ApiRequestException(string message) : base(message)
        {
        }

        public ApiRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ApiRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

using System;

namespace MessageSender.Model.Http
{
    public class ApiException: Exception
    {
        public int Code { get; set; }
    }
}

using System;

namespace MessageSender.Model.Http
{
    public class ApiException : Exception
    {
        public ApiException(ApiErrorResponse apiErrorResponse): 
            base(apiErrorResponse.Message)
        {
            Code = apiErrorResponse.Code;
        }

        public int Code { get; set; }
    }
    public class ApiErrorResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}

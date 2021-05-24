using Common.Exceptions;

namespace Common.Dto
{
    public class ApiError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public ApiError()
        {
            Code = 500;
            Message =  Lang.Lang.InternalServerError;
        }
        public ApiError(HttpException httpException)
        {
            Code = httpException.StatusCode;
            Message = httpException.Message;
        }
    }
}
